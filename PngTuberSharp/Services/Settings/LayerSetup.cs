﻿using CommunityToolkit.Mvvm.ComponentModel;
using GlobalHotKeys.Native.Types;
using PngTuberSharp.Layers;
using PngTuberSharp.Layers.Microphone;
using PngTuberSharp.Services.Hotkey;
using PngTuberSharp.Services.Twitch;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PngTuberSharp.Services.Settings
{
    public partial class LayerSetup : ObservableObject
    {
        private List<Action> callbacks = new();

        [ObservableProperty]
        private bool showFPS = true;

        public List<Layersetting> Layers { get; set; } = new()
        {
            new Layersetting()
            {
                Name = "Rotate",
                Layers = [new RotateByRel()],
                Trigger = new HotkeyTrigger() { VirtualKeyCode = VirtualKeyCode.VK_F11 },
            },
        };

        public void ApplySettings()
        {
            CleanUp();
            foreach (var item in Layers)
            {
                switch (item.Trigger)
                {
                    case AlwaysActive:
                        item.AddLayers();
                        break;
                    case HotkeyTrigger hotKey:
                        var callback = () => item.AddLayers();
                        WinHotkey.AddHotkey(hotKey.VirtualKeyCode, hotKey.Modifiers, callback);
                        callbacks.Add(callback);
                        break;
                    case TwitchRedeem redeem:
                        TwitchEventSocket.RedeemUsed += redeem.Triggered;
                        break;
                    case TwitchBits bits:
                        TwitchEventSocket.BitsUsed += bits.Triggered;
                        break;
                    default:
                        break;
                }
            }
        }

        public void CleanUp()
        {
            WinHotkey.RemoveCallbacks(callbacks);
            callbacks.Clear();

            foreach (var item in Layers.Where(x => x.Trigger is TwitchRedeem))
            {
                TwitchEventSocket.RedeemUsed -= ((TwitchRedeem)item.Trigger).Triggered;
            }

            foreach (var item in Layers.Where(x => x.Trigger is TwitchBits))
            {
                TwitchEventSocket.BitsUsed -= ((TwitchBits)item.Trigger).Triggered;
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

        public void Cleanup()
        {
            switch (Trigger)
            {
                case TwitchRedeem redeem:
                    TwitchEventSocket.RedeemUsed -= redeem.Triggered;
                    break;
                case TwitchBits redeem:
                    TwitchEventSocket.BitsUsed -= redeem.Triggered;
                    break;
                default:
                    break;
            }
        }
    }


}
