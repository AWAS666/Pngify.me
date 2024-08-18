using GlobalHotKeys.Native.Types;
using System;
using System.Text.Json.Serialization;

namespace PngTuberSharp.Services.Settings
{
    public abstract class Trigger
    {
        [JsonIgnore]
        public Layersetting Parent { get; internal set; }

    }
    public class AlwaysActive : Trigger
    {

    }

    public class HotkeyTrigger : Trigger
    {
        public VirtualKeyCode VirtualKeyCode { get; set; }
        public Modifiers Modifiers { get; set; }
    }

    public class TwitchTrigger : Trigger
    {
        public string Redeem { get; set; }

        public void Triggered(object? sender, string e)
        {
            if (string.Compare(Redeem, e, StringComparison.OrdinalIgnoreCase) == 0)
            {
                Parent.AddLayers();
            }
        }
    }
}
