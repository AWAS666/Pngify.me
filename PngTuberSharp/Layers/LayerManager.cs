using PngTuberSharp.Services;
using PngTuberSharp.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PngTuberSharp.Layers
{
    public static class LayerManager
    {
        private static Task tickLoop;
        public static List<BaseLayer> Layers { get; set; } = new List<BaseLayer>();
        public static float Time { get; private set; }

        public static int UpdateInterval { get; private set; } = 1000 / 60;
        public static int TotalRunTime { get; private set; }

        public static EventHandler<LayerValues> ValueUpdate;
        public static EventHandler<BaseLayer> NewLayer;

        static LayerManager()
        {
            tickLoop = Task.Run(TickLoop);
            TwitchHandler.RedeemUsed += TwitchRedeem;
        }

        private static void TwitchRedeem(object? sender, string e)
        {
            var layers = SettingsManager.Current.LayerSetup.Layers.Where(x => x.Trigger is TwitchTrigger);
            foreach (var layer in layers)
            {
                // todo clone layer here
                AddLayer(layer.Layer.Clone());
            }
        }

        private static async Task TickLoop()
        {
            TotalRunTime = 0;
            while (true)
            {
                Update(UpdateInterval / 1000f);
                await Task.Delay(UpdateInterval);
                TotalRunTime += UpdateInterval;
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
            foreach (BaseLayer layer in Layers)
            {
                layer.OnCalculateParameters(dt, ref layert);
            }
            ValueUpdate?.Invoke(null, layert);
        }
    }
}
