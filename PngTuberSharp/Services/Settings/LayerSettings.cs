using GlobalHotKeys.Native.Types;
using PngTuberSharp.Layers;
using System.Collections.Generic;

namespace PngTuberSharp.Services.Settings
{
    public class LayerSettings
    {
        public List<Layersetting> Layers { get; set; } = new();
    }

    public class Layersetting
    {
        public string Name { get; set; }
        public List<BaseLayer> Layers { get; set; }
        public Trigger Trigger { get; set; }

    }

    public class AlwaysActive() : Trigger
    {
    }

    public class HotkeyTrigger() : Trigger
    {
        public VirtualKeyCode VirtualKeyCode { get; set; }
        public Modifiers Modifiers { get; set; }
    }

    public class TwitchTrigger() : Trigger
    {
        public string Redeem { get; set; }
    }

    public abstract class Trigger()
    {

    }
}
