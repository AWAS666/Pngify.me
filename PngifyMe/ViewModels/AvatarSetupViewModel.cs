using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Layers;
using PngifyMe.Services;
using PngifyMe.Services.CharacterSetup;
using PngifyMe.Services.CharacterSetup.Advanced;
using PngifyMe.Services.CharacterSetup.Basic;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;


namespace PngifyMe.ViewModels;

/// <summary>
/// todo: this class needs to be split more, multiple vm versions for different setups
/// </summary>
public partial class AvatarSetupViewModel : ObservableObject
{
    public Func<IStorageProvider> GetStorageProvider { get; }

    [ObservableProperty]
    private string selectedMode;

    [ObservableProperty]
    private object selectedView;

    public ObservableCollection<string> Options { get; } = new ObservableCollection<string>
    {
        "Basic",
        "Sprite (Advanced)"
    };

    public AvatarSetupViewModel() : this(null)
    {

    }

    public AvatarSetupViewModel(Func<IStorageProvider> getStorage)
    {
        GetStorageProvider = getStorage;
        switch (SettingsManager.Current.Profile.Active.AvatarSettings)
        {
            case BasicCharSettings:
                SelectedMode = Options.First();
                break;
            case SpriteCharacterSettings:
                SelectedMode = Options.Last();
                break;
            default:
                break;
        }
        SettingsManager.Current.Profile.PropertyChanged += Profile_PropertyChanged;
    }

    private void Profile_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        CheckSettings();
    }

    partial void OnSelectedModeChanged(string? oldValue, string newValue)
    {
        if (oldValue != null)
            LayerManager.CharacterStateHandler.ChangeSetup(newValue);
        CheckSettings();
    }

    private void CheckSettings()
    {
        switch (SettingsManager.Current.Profile.Active.AvatarSettings)
        {
            case BasicCharSettings:
                SelectedView = new BasicSetupViewModel(GetStorageProvider);
                break;
            case SpriteCharacterSettings:
                SelectedView = new SpriteSetupViewModel(GetStorageProvider);
                break;
            default:
                break;
        }
    }
}
