using PngTuberSharp.Layers.Microphone;
using PngTuberSharp.Services;
using PngTuberSharp.Services.Settings;
using PngTuberSharp.Services.ThrowingSystem;
using PngTuberSharp.Services.Twitch;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PngTuberSharp.Layers
{
    public static class LayerManager
    {
        private static Task tickLoop;
        public static List<BaseLayer> Layers { get; set; } = new List<BaseLayer>();
        public static float Time { get; private set; }

        public static float UpdateInterval => 1.0f / FPS;
        public static int FPS { get; private set; } = 60;
        public static float TotalRunTime { get; private set; }

        public static EventHandler<LayerValues> ValueUpdate;
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
            while (true)
            {
                var watch = new Stopwatch();
                watch.Start();
                Update(UpdateInterval + delay);

                Debug.WriteLine($"Position code took: {watch.ElapsedMilliseconds} ms");
                double time = UpdateInterval * 1000f - watch.Elapsed.TotalMilliseconds;

                // todo fix to more accurate timer
                //await Task.Delay(Math.Max(1, time));
                await Delay(Math.Max(1, time));               

                TotalRunTime += UpdateInterval;
                FPSUpdate?.Invoke(null, (float)(1f / watch.Elapsed.TotalMilliseconds * 1000f));
                delay = (float)(watch.Elapsed.TotalMilliseconds / 1000f - UpdateInterval);
                Debug.WriteLine($"Total took: {watch.ElapsedMilliseconds} ms");
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
                await Task.Yield();
                //Thread.Sleep(0); // setting at least 1 here would involve a timer which we don't want to
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
                foreach (BaseLayer layer in Layers)
                {
                    layer.OnCalculateParameters(dt, ref layert);
                }

                //ThrowingSystem.SwapImage(layert.Image);
                //ThrowingSystem.Update(dt, ref layert);

                ValueUpdate?.Invoke(null, layert);

            }
            catch (Exception e)
            {
                Log.Error(e, $"Error in LayerManagerupdate: {e.Message}");
            }
        }
    }
}
