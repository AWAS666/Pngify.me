using Newtonsoft.Json.Linq;
using PngifyMe.Layers.Microphone;
using PngifyMe.Services;
using PngifyMe.Services.ThrowingSystem;
using Serilog;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace PngifyMe.Layers
{
    public static class LayerManager
    {
        private static Task tickLoop;
        public static List<BaseLayer> Layers { get; set; } = new List<BaseLayer>();
        public static float Time { get; private set; }

        public static float UpdateInterval => 1.0f / SettingsManager.Current.LayerSetup.TargetFPS;
        //public static int FPS { get; private set; } = 120;
        public static float TotalRunTime { get; private set; }

        public static EventHandler<LayerValues> ValueUpdate;
        public static EventHandler<SKImage> ImageUpdate;
        public static EventHandler<BaseLayer> NewLayer;
        public static EventHandler<float> FPSUpdate;

        public static MicroPhoneStateLayer MicroPhoneStateLayer { get; private set; } = new MicroPhoneStateLayer();
        public static ThrowingSystem ThrowingSystem { get; private set; } = new ThrowingSystem();

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

        public static void AddLayer(BaseLayer layer)
        {
            // check if this is a unique layer
            if (layer.Unique)
            {
                if (Layers.Any(x => x.GetType() == layer.GetType()))
                    return;
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

                var layert = new LayerValues();
                MicroPhoneStateLayer.Update(dt, ref layert);
                foreach (BaseLayer layer in Layers.ToList())
                {
                    layer.OnCalculateParameters(dt, ref layert);
                }

                var draw = UpdateThrowingSystem(dt, ref layert);

                ValueUpdate?.Invoke(null, layert);
                ImageUpdate?.Invoke(null, draw);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Error in LayerManagerupdate: {e.Message}");
            }
        }

        private static SKImage UpdateThrowingSystem(float dt, ref LayerValues layert)
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
            int width = 1920;
            int height = 1080;
            using var mainBitmap = new SKBitmap(width, height);
            using (SKCanvas canvas = new SKCanvas(mainBitmap))
            {
                // Clear canvas with white color
                canvas.Clear();

                // Define the rotation angle in degrees
                float rotationAngle = layert.Rotation; // Adjust the angle as needed

                // Define the zoom factor (1.0 means no zoom, 2.0 means 2x zoom, etc.)
                float zoomFactor = layert.ZoomX; // Adjust the zoom factor as needed

                // Define the opacity (1.0 means fully opaque, 0.0 means fully transparent)
                float opacity = layert.Opacity; // Adjust the opacity as needed

                // Calculate new dimensions based on zoom factor
                int zoomedWidth = (int)(baseImg.Width * zoomFactor);
                int zoomedHeight = (int)(baseImg.Height * zoomFactor);

                // Create a new SKBitmap for the rotated, zoomed, and opaque image
                using var transformedImage = new SKBitmap(zoomedWidth, zoomedHeight);

                using (SKCanvas transformedCanvas = new SKCanvas(transformedImage))
                {
                    // Set the pivot point for rotation and zoom to the center of the image
                    transformedCanvas.Translate(zoomedWidth / 2, zoomedHeight / 2);
                    transformedCanvas.Scale(zoomFactor);
                    transformedCanvas.RotateDegrees(rotationAngle);
                    transformedCanvas.Translate(-baseImg.Width / 2, -baseImg.Height / 2);

                    // Set opacity
                    using (SKPaint paint = new SKPaint { Color = SKColors.White.WithAlpha((byte)(opacity * 255)) })
                    {
                        transformedCanvas.DrawBitmap(baseImg, 0, 0, paint);
                    }
                }

                layert.PosX += (width - baseImg.Width) / 2;

                // Draw the main bitmap on the canvas
                canvas.DrawBitmap(transformedImage, layert.PosX, layert.PosY);

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
            }
            return SKImage.FromBitmap(mainBitmap);
        }
    }
}
