using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Layers;
using PngifyMe.Services.Helpers;
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
    private TriggerRegistrationHelper triggerHelper = new();

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
        triggerHelper.Cleanup();
        LayerManager.Layers.Clear();
        foreach (var item in Layers)
        {
            if (item.Trigger is AlwaysActive)
            {
                item.AddLayers();
            }
            else
            {
                triggerHelper.RegisterTrigger(item.Trigger, item.AddLayers);
            }
        }
    }

    public void CleanUp()
    {
        triggerHelper.Cleanup();
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
        TriggerRegistrationHelper.UnregisterSingleTrigger(Trigger);
    }
}
