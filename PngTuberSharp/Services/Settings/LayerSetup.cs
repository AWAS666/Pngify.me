using GlobalHotKeys.Native.Types;
using PngTuberSharp.Layers;
using PngTuberSharp.Layers.Microphone;
using PngTuberSharp.Services.Hotkey;
using PngTuberSharp.Services.Twitch;
using System;
using System.Collections.Generic;

namespace PngTuberSharp.Services.Settings
{
    public class LayerSetup
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
        public List<BaseLayer> Layers { get; set; } = new();
        public Trigger Trigger
        {
            get => trigger;
            set
            {
                trigger = value;
                trigger.Parent = this;
            }
        }
        private Trigger trigger = new AlwaysActive();

        public void AddLayers()
        {
            foreach (var layer in Layers)
            {
                LayerManager.AddLayer(layer.Clone());
            }
        }
    }


}
