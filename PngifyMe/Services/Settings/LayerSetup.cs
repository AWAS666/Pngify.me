using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Layers;
using PngifyMe.Services.Hotkey;
using PngifyMe.Services.Twitch;
using SharpHook.Data;
using SharpHook.Native;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PngifyMe.Services.Settings;

public partial class LayerSetup : ObservableObject
{
    private List<Action> callbacks = new();

    [ObservableProperty]
    private bool showFPS = false;

    public List<Layersetting> Layers { get; set; } = new()
    {
        new Layersetting()
        {
            Name = "Basic",
            Layers = [new WiggleOnTalk(), new SquishOnTalk(), new IdleMove()],
            Trigger = new AlwaysActive(),
        },
        new Layersetting()
        {
            Name = "Rotate",
            Layers = [new RotateByRel()],
            Trigger = new HotkeyTrigger() { VirtualKeyCode = KeyCode.Vc1, Modifiers =  EventMask.LeftCtrl },
        },
    };

    public void ApplySettings()
    {
        CleanUp();
        LayerManager.Layers.Clear();
        foreach (var item in Layers)
        {
            item.Trigger.Callback = item.AddLayers;
            switch (item.Trigger)
            {
                case AlwaysActive:
                    item.AddLayers();
                    break;
                case HotkeyTrigger hotKey:
                    var callback = () => item.AddLayers();
                    HotkeyManager.AddHotkey(hotKey.VirtualKeyCode, hotKey.Modifiers, callback);
                    callbacks.Add(callback);
                    break;
                case TwitchRedeem redeem:
                    TwitchEventSocket.RedeemUsed += redeem.Triggered;
                    break;
                case TwitchBits bits:
                    TwitchEventSocket.BitsUsed += bits.Triggered;
                    break;
                case TwitchSub subs:
                    TwitchEventSocket.AnySub += subs.Triggered;
                    break;
                case TwitchTextCommand command:
                    TwitchEventSocket.NewChat += command.Triggered;
                    break;
                default:
                    break;
            }
        }
    }

    public void CleanUp()
    {
        HotkeyManager.RemoveCallbacks(callbacks);
        callbacks.Clear();

        foreach (var item in Layers.Where(x => x.Trigger is TwitchRedeem))
        {
            TwitchEventSocket.RedeemUsed -= ((TwitchRedeem)item.Trigger).Triggered;
        }

        foreach (var item in Layers.Where(x => x.Trigger is TwitchBits))
        {
            TwitchEventSocket.BitsUsed -= ((TwitchBits)item.Trigger).Triggered;
        }

        foreach (var item in Layers.Where(x => x.Trigger is TwitchSub))
        {
            TwitchEventSocket.AnySub -= ((TwitchSub)item.Trigger).Triggered;
        }

        foreach (var item in Layers.Where(x => x.Trigger is TwitchTextCommand))
        {
            TwitchEventSocket.NewChat -= ((TwitchTextCommand)item.Trigger).Triggered;
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
        }
    }
    private Trigger trigger = new AlwaysActive();

    public void AddLayers()
    {
        foreach (var layer in Layers)
        {
            LayerManager.AddLayer(layer.Clone(this), Trigger.IsToggleable);
        }
        LayerManager.LayerTriggered?.Invoke(this, this);
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
            case TwitchSub sub:
                TwitchEventSocket.AnySub -= sub.Triggered;
                break;
            case TwitchTextCommand command:
                TwitchEventSocket.NewChat -= command.Triggered;
                break;
            default:
                break;
        }
    }
}
