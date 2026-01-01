using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Services.Helpers;
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
    private TriggerRegistrationHelper triggerHelper = new();

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
    private double blinkIntervalVariance = 0f;

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
        triggerHelper.Cleanup();
        triggerHelper.RegisterTriggers(States, item => () =>
        {
            ActivateState = item;
        });
    }

    private void CleanUp()
    {
        triggerHelper.Cleanup();
    }


}
