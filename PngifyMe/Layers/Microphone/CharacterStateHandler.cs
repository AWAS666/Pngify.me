using PngifyMe.Services;
using PngifyMe.Services.CharacterSetup;
using PngifyMe.Services.CharacterSetup.Advanced;
using PngifyMe.Services.CharacterSetup.Basic;
using PngifyMe.Services.CharacterSetup.Images;
using System;

namespace PngifyMe.Layers.Microphone;

public class CharacterStateHandler
{
    public ICharacterSetup CharacterSetup { get; private set; }

    public BaseImage CurrentImage => CharacterSetup.CurrentImage;

    public EventHandler<CharacterState> StateChanged; // dead for now, todo

    public CharacterStateHandler()
    {
        switch (SettingsManager.Current.Profile.Active.AvatarSettings)
        {
            case BasicCharSettings:
                CharacterSetup = new BasicCharacterSetup();
                break;
            case SpriteCharacterSettings:
                CharacterSetup = new SpriteCharacterSetup();
                break;
            default:
                break;
        }

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
        CharacterSetup.Settings = SettingsManager.Current.Profile.Active.AvatarSettings;
        CharacterSetup.RefreshCharacterSettings();
    }

    public void Update(float dt, ref LayerValues values)
    {
        if (CharacterSetup.Settings == null) return;
        CharacterSetup.Update(dt, ref values);
    }

    public void ToggleState(string stateName)
    {
        CharacterSetup.ToggleState(stateName);
    }

    internal void SetupHotKeys()
    {
        CharacterSetup.SetupHotKeys();
    }

    internal void ChangeSetup(string newValue)
    {
        LayerManager.Pause();
        switch (newValue)
        {
            case "Basic":
                CharacterSetup = new BasicCharacterSetup();
                SettingsManager.Current.Profile.Active.AvatarSettings = new BasicCharSettings();
                break;
            case "Sprite (Advanced)":
                CharacterSetup = new SpriteCharacterSetup();
                SettingsManager.Current.Profile.Active.AvatarSettings = new SpriteCharacterSettings();
                break;
            default:
                break;
        }
        RefreshCharacterSettings();
        LayerManager.UnPause();
    }
}
