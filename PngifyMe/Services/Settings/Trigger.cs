using GlobalHotKeys.Native.Types;
using System;
using System.Text.Json.Serialization;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;

namespace PngifyMe.Services.Settings
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

    public class TwitchRedeem : Trigger
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

    public class TwitchBits : Trigger
    {
        public uint MinAmount { get; set; }
        public uint MaxAmount { get; set; } = uint.MaxValue;

        internal void Triggered(object? sender, ChannelCheer e)
        {
            if (e.Bits >= MinAmount && e.Bits < MaxAmount)
            {
                Parent.AddLayers();
            }
        }
    }
}
