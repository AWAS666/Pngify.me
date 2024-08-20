﻿using PngTuberSharp.Layers.Microphone;
using PngTuberSharp.Services;
using PngTuberSharp.Services.Settings;
using PngTuberSharp.Services.Twitch;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static MicroPhoneStateLayer MicroPhoneStateLayer { get; private set; } = new MicroPhoneStateLayer();

        static LayerManager()
        {
            tickLoop = Task.Run(TickLoop);
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
            var watch = new Stopwatch();
            watch.Start();
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
            ValueUpdate?.Invoke(null, layert);
            Log.Debug($"Position code took: {watch.ElapsedMilliseconds} ms");
        }        
    }
}
