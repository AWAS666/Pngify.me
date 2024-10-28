using PngifyMe.Layers.Microphone;
using PngifyMe.Services;
using PngifyMe.Services.ThrowingSystem;
using Serilog;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PngifyMe.Layers
{
    public static class LayerManager
    {
        private static Task tickLoop;
        public static List<BaseLayer> Layers { get; set; } = new List<BaseLayer>();
        public static List<BaseLayer> RenderedLayers { get; set; } = new List<BaseLayer>();
        public static float Time { get; private set; }

        public static float UpdateInterval
        {
            get
            {
                return 1.0f / Specsmanager.FPS;
            }
        }
        //public static int FPS { get; private set; } = 120;
        public static float TotalRunTime { get; private set; }

        public static EventHandler<LayerValues> ValueUpdate;
        /// <summary>
        /// doesnt hold an image anymore as that has issues with gc/dispose
        /// </summary>
        public static EventHandler<SKImage> ImageUpdate;
        public static EventHandler<BaseLayer> NewLayer;
        public static EventHandler<float> FPSUpdate;

        public static MicroPhoneStateLayer MicroPhoneStateLayer { get; private set; } = new MicroPhoneStateLayer();
        public static ThrowingSystem ThrowingSystem { get; private set; } = new ThrowingSystem();
        public static SKBitmap CurrentFrame { get; private set; }

        /// <summary>
        /// need to buffer frames, as if they are disposed immediatly, there will be access violations
        /// set to 10
        /// </summary>
        public static List<SKBitmap> FrameBuffer { get; private set; } = new();

        static LayerManager()
        {
            tickLoop = Task.Run(TickLoop);
        }

        private static async Task TickLoop()
        {
            float delay = 0f;
            TotalRunTime = 0;
            SettingsManager.Current.LayerSetup.ApplySettings();
            while (true)
            {
                var watch = new Stopwatch();
                watch.Start();
                Update(UpdateInterval + delay);

                //Debug.WriteLine($"Position code took: {watch.ElapsedMilliseconds} ms");
                double time = UpdateInterval * 1000f - watch.Elapsed.TotalMilliseconds;

                // todo fix to more accurate timer
                //await Delay(Math.Max(1, time));
                await Task.Delay((int)Math.Max(1, time - 16));

                TotalRunTime += UpdateInterval;
                FPSUpdate?.Invoke(null, (float)(1f / watch.Elapsed.TotalMilliseconds * 1000f));
                delay = (float)(watch.Elapsed.TotalMilliseconds / 1000f - UpdateInterval);
                TotalRunTime += delay;
                //Debug.WriteLine($"Total took: {watch.ElapsedMilliseconds} ms");
            }
        }

        private static async Task Delay(double ms)
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            while (true)
            {
                if (sw.Elapsed.TotalMilliseconds >= ms)
                {
                    return;
                }
                //await Task.Yield();
                // setting at least 1 here would involve a timer which we don't want to
                Thread.Sleep(0);
                // thread sleep takes only half as much cpu as task.delay, but limits fps more
                // todo find better option for doing this
            }
        }

        public static void AddLayer(BaseLayer layer, bool isToggable)
        {
            // check if this is a unique layer
            if (layer.Unique)
            {
                if (Layers.Any(x => x.GetType() == layer.GetType()))
                    return;
            }
            if (isToggable)
            {
                // check if any matches and have it exit instead of adding a new one
                var exists = Layers.FirstOrDefault(x => x.GetType() == layer.GetType() && x.AddedBy == layer.AddedBy);
                if (exists != null)
                {
                    exists.IsExiting = true;
                    return;
                }
            }
            Layers.Add(layer);
            layer.OnEnter();
            NewLayer?.Invoke(null, layer);
        }

        public static void Update(float dt)
        {
            try
            {
                Time += dt;
                foreach (BaseLayer layer in Layers.ToList())
                {
                    bool exit = layer.Update(dt);
                    if (exit)
                    {
                        layer.OnExit();
                        Layers.Remove(layer);
                    }
                }

                RenderedLayers = Layers.ToList();

                var layert = new LayerValues();
                MicroPhoneStateLayer.Update(dt, ref layert);
                foreach (BaseLayer layer in RenderedLayers)
                {
                    layer.OnCalculateParameters(dt, ref layert);
                }

                UpdateThrowingSystem(dt, ref layert);

                ValueUpdate?.Invoke(null, layert);
                ImageUpdate?.Invoke(null, null);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Error in LayerManagerupdate: {e.Message}");
            }
        }

        private static void UpdateThrowingSystem(float dt, ref LayerValues layert)
        {
            ThrowingSystem.SwapImage(layert.Image, layert);
            if (SettingsManager.Current.Tits.Enabled)
            {
                var watch = new Stopwatch();
                watch.Start();
                ThrowingSystem.Update(dt, ref layert);
                //Debug.WriteLine($"Tits took: {watch.Elapsed.TotalMilliseconds}ms");
            }

            var baseImg = layert.Image;
            int width = Specsmanager.Width;
            int height = Specsmanager.Height;
            var mainBitmap = new SKBitmap(width, height);
            using (SKCanvas canvas = new SKCanvas(mainBitmap))
            {

                float rotationAngle = layert.Rotation;
                float opacity = layert.Opacity;

                canvas.Save();
                canvas.Translate(layert.PosX, layert.PosY);
                canvas.Translate(width / 2, height / 2);
                canvas.RotateDegrees((float)rotationAngle);
                canvas.Scale(layert.ZoomX, layert.ZoomY);
                canvas.Translate(-width / 2, -height / 2);

                // Apply transformations directly to the main canvas
                using (SKPaint paint = new SKPaint { Color = SKColors.White.WithAlpha((byte)(opacity * 255)) })
                {
                    canvas.DrawBitmap(baseImg,
                        width / 2 - baseImg.Width / 2,
                        height / 2 - baseImg.Height / 2,
                        paint);
                }

                if (SettingsManager.Current.Profile.Active.MicroPhone.TransitionTime != 0f)
                    if (MicroPhoneStateLayer.BlendTime != null && MicroPhoneStateLayer.LastImage != null)
                    {
                        var blend = (MicroPhoneStateLayer.BlendTime - MicroPhoneStateLayer.CurrentTime) / SettingsManager.Current.Profile.Active.MicroPhone.TransitionTime;
                        // factor in blending of main layer
                        blend *= opacity;
                        using (SKPaint paint = new SKPaint { Color = SKColors.White.WithAlpha((byte)(blend * 255)) })
                        {
                            canvas.DrawBitmap(MicroPhoneStateLayer.LastImage.GetImage(TimeSpan.FromSeconds(MicroPhoneStateLayer.CurrentTime)),
                                width / 2 - baseImg.Width / 2,
                                height / 2 - baseImg.Height / 2,
                                paint);
                        }
                    }

                foreach (ImageLayer img in RenderedLayers.Where(x => x is ImageLayer).Cast<ImageLayer>().Where(x => x.ApplyOtherEffects))
                {
                    img.RenderImage(canvas, layert.PosX, layert.PosY);
                }

                canvas.Restore();

                if (SettingsManager.Current.Tits.HitLinesVisible)
                {
                    using (var paint = new SKPaint
                    {
                        Color = SKColors.Red,
                        StrokeWidth = 2,
                        IsAntialias = true,
                        Style = SKPaintStyle.Stroke,
                    })
                    {
                        ThrowingSystem.MainBody.Collision.DrawOutlines(canvas, paint);
                    }
                }

                // Draw moving objects from ThrowingSystem.Objects
                if (SettingsManager.Current.Tits.Enabled)
                {
                    foreach (var obj in ThrowingSystem.Objects.ToList())
                    {
                        // Create a new SKBitmap for the rotated, zoomed, and opaque image
                        using var rotobj = new SKBitmap(obj.Image.Width, obj.Image.Height);

                        using (SKCanvas rotatedCanvas = new SKCanvas(rotobj))
                        {
                            // Set the pivot point for rotation and zoom to the center of the image
                            rotatedCanvas.Translate(obj.Image.Width / 2, obj.Image.Height / 2);
                            rotatedCanvas.RotateDegrees(obj.Rotation);
                            rotatedCanvas.Translate(-obj.Image.Width / 2, -obj.Image.Height / 2);

                            rotatedCanvas.DrawBitmap(obj.Image, 0, 0);
                        }

                        canvas.DrawBitmap(rotobj, obj.X, obj.Y);

                        if (SettingsManager.Current.Tits.HitLinesVisible)
                        {
                            using (var paint = new SKPaint
                            {
                                Color = SKColors.Blue,
                                StrokeWidth = 2,
                                IsAntialias = true,
                                Style = SKPaintStyle.Stroke,

                            })
                            {
                                obj.Collision.DrawOutlines(canvas, paint);
                            }
                        }
                    }
                }

                foreach (ImageLayer img in RenderedLayers.Where(x => x is ImageLayer).Cast<ImageLayer>().Where(x => !x.ApplyOtherEffects))
                {
                    img.RenderImage(canvas, 0, 0);
                }
            }
            CurrentFrame = mainBitmap;
            FrameBuffer.Add(CurrentFrame);
            foreach (var item in FrameBuffer.SkipLast(10))
            {
                item.Dispose();
                FrameBuffer.Remove(item);
            }
        }
    }
}
