using PngifyMe.Services;
using PngifyMe.Services.CharacterSetup;
using PngifyMe.Services.CharacterSetup.Basic;
using PngifyMe.Services.CharacterSetup.Images;
using PngifyMe.Services.Hotkey;
using PngifyMe.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;

namespace PngifyMe.Layers.Microphone;

public class CharacterStateHandler
{
    public ICharacterSetup CharacterSetup { get; } = new BasicCharacterSetup();

    public BaseImage CurrentImage => CharacterSetup.CurrentImage;

    public EventHandler<CharacterState> StateChanged; // dead for now, todo

    public CharacterStateHandler()
    {
        RefreshCharacterSettings();
        SettingsManager.Current.Profile.PropertyChanged += Profile_PropertyChanged;
    }

    private void Profile_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        RefreshCharacterSettings();
    }

    private void RefreshCharacterSettings()
    {
        //micSettings = SettingsManager.Current.Profile.Active.CharacterSetup;
        CharacterSetup.Settings = SettingsManager.Current.Profile.Active.CharacterSetup;
        CharacterSetup.RefreshCharacterSettings();
    }

    public void Update(float dt, ref LayerValues values)
    {
        CharacterSetup.Update(dt, ref values);
    }

    public void ToggleState(CharacterState state)
    {
        CharacterSetup.ToggleState(state);
    }

    internal void SetupHotKeys()
    {
        CharacterSetup.SetupHotKeys();
    }
}
