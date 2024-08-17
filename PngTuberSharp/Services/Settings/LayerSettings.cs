using GlobalHotKeys.Native.Types;
using PngTuberSharp.Layers;
using PngTuberSharp.Layers.Microphone;
using PngTuberSharp.Services.Hotkey;
using PngTuberSharp.Services.Twitch;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PngTuberSharp.Services.Settings
{
    public class LayerSettings
    {
        public List<Layersetting> Layers { get; set; } = new()
        {
            new Layersetting()
            {
                Name = "Default Mic",
                Layers = [new BasicMicroPhoneLayer()],
                Trigger = new AlwaysActive()
            },
            new Layersetting()
            {
                Name = "Rotate",
                Layers = [new RotateByRel()],
                Trigger = new HotkeyTrigger() { VirtualKeyCode = VirtualKeyCode.VK_F11 },
            },
        };

        public void ApplySettings()
        {
            foreach (var item in Layers)
            {
                switch (item.Trigger)
                {
                    case AlwaysActive:
                        item.AddLayers();
                        break;
                    case HotkeyTrigger hotKey:
                        WinHotkey.AddHotkey(hotKey.VirtualKeyCode, hotKey.Modifiers, item.AddLayers);
                        break;

                    case TwitchTrigger redeem:
                        TwitchEventSocket.RedeemUsed += redeem.Triggered;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public class Layersetting
    {
        public string Name { get; set; }
        public List<BaseLayer> Layers { get; set; }
        public Trigger Trigger
        {
            get => trigger;
            set
            {
                trigger = value;
                trigger.Parent = this;
            }
        }
        private Trigger trigger;

        public void AddLayers()
        {
            foreach (var layer in Layers)
            {
                LayerManager.AddLayer(layer.Clone());
            }
        }
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

    public abstract class Trigger
    {
        [JsonIgnore]
        public Layersetting Parent { get; internal set; }

    }
}
