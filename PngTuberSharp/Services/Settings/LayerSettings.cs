using Avalonia.Input;
using PngTuberSharp.Layers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
namespace PngTuberSharp.Services.Settings
{
    public class LayerSettings
    {
        public List<Layersetting> Layers { get; set; } = new();       
    }

    public class Layersetting
    {
        public string Name { get; set; }
        public BaseLayer Layer { get; set; }
        public Trigger Trigger { get; set; }

    }

    public class AlwaysActive() : Trigger
    {
    }

    public class HotkeyTrigger() : Trigger
    {
    }

    public class TwitchTrigger() : Trigger
    {
        public string Redeem { get; set; }
    }

    public abstract class Trigger()
    {

    }
}
