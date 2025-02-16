using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Services.Hotkey;
using PngifyMe.Services.Settings;
using PngifyMe.Services.Twitch;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;


namespace PngifyMe.Services.CharacterSetup.Advanced;
public partial class SpriteCharacterSettings : ObservableObject, IAvatarSettings
{
    private List<Action> callbacks = new();

    [ObservableProperty]
    private List<SpriteImage> spriteImages;

    [ObservableProperty]
    private SpriteImage parent = new();

    [property: JsonIgnore]
    [ObservableProperty]
    private SpriteImage? selected = null;

    [ObservableProperty]
    private double blinkTime = 0.25f;

    [ObservableProperty]
    private double blinkInterval = 3f;

    [ObservableProperty]
    private float offsetX = 0f;

    [ObservableProperty]
    private float offsetY = 0f;

    [ObservableProperty]
    private float zoom = 0.9f;

    [property: JsonIgnore]
    [ObservableProperty]
    private SpriteStates activateState;
    public ObservableCollection<SpriteStates> States { get; set; } = [new SpriteStates() {
        Name = "Default",
    }];
    public List<string> AvailableStates() => States.Select(s => s.Name).ToList();

    public SpriteCharacterSettings()
    {
        SpriteImages = [parent];
        if (States.Count > 0)
            ActivateState = States.First();
        else
            ActivateState = new();
    }

    public void SetupTriggers()
    {
        CleanUp();
        foreach (var item in States)
        {
            var callback = () =>
            {
                ActivateState = item;
            };
            item.Trigger.Callback = callback;
            switch (item.Trigger)
            {
                case HotkeyTrigger hotKey:
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
                default:
                    break;
            }
        }
    }

    private void CleanUp()
    {
        HotkeyManager.RemoveCallbacks(callbacks);
        callbacks.Clear();

        foreach (var item in States)
        {
            switch (item.Trigger)
            {
                case TwitchRedeem redeem:
                    TwitchEventSocket.RedeemUsed -= redeem.Triggered;
                    break;
                case TwitchBits bits:
                    TwitchEventSocket.BitsUsed -= bits.Triggered;
                    break;
                case TwitchSub subs:
                    TwitchEventSocket.AnySub -= subs.Triggered;
                    break;
            }
        }
    }


}
