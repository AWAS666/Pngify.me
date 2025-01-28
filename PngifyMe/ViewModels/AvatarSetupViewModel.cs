using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PngifyMe.Layers;
using PngifyMe.Services;
using PngifyMe.Services.CharacterSetup;
using PngifyMe.Services.CharacterSetup.Advanced;
using PngifyMe.Services.CharacterSetup.Basic;
using PngifyMe.Services.Settings;
using PngifyMe.ViewModels.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;


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
        switch (SettingsManager.Current.Profile.Active.CharacterSetup)
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
        switch (SettingsManager.Current.Profile.Active.CharacterSetup)
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

public partial class BasicSetupViewModel : ObservableObject
{
    public Func<IStorageProvider> GetStorageProvider { get; }

    private List<CharacterState> baseStates;
    public BasicCharSettings Settings { get; }
    public MicSettings MicSettings { get; }

    [ObservableProperty]
    private ObservableCollection<BasicStateViewModel> states;

    public BasicSetupViewModel() : this(null)
    {

    }

    public BasicSetupViewModel(Func<IStorageProvider> getStorage)
    {
        GetStorageProvider = getStorage;
        baseStates = SettingsManager.Current.Profile.Active.CharacterSetup.States;
        Settings = (BasicCharSettings)SettingsManager.Current.Profile.Active.CharacterSetup;
        MicSettings = SettingsManager.Current.Profile.Active.MicSettings;
        states = new ObservableCollection<BasicStateViewModel>(baseStates.Select(x => new BasicStateViewModel(x, this)));
    }

    public void Add()
    {
        var set = new CharacterState();
        States.Add(new BasicStateViewModel(set, this));
        baseStates.Add(set);
    }

    public void Remove(BasicStateViewModel vm)
    {
        // dont allow removal of final state
        if (States.Count <= 1)
            return;
        States.Remove(vm);
        baseStates.Remove(vm.State);
    }

    public void SwitchState(BasicStateViewModel vm)
    {
        LayerManager.CharacterStateHandler.ToggleState(vm.State);
    }

    public void Apply()
    {
        SettingsManager.Save();
        LayerManager.CharacterStateHandler.SetupHotKeys();
    }
}

public partial class SpriteSetupViewModel : ObservableObject
{
    public Func<IStorageProvider> GetStorageProvider { get; }
    public SpriteCharacterSettings Settings { get; }

    public SpriteSetupViewModel() : this(null)
    {

    }

    public SpriteSetupViewModel(Func<IStorageProvider> getStorage)
    {
        GetStorageProvider = getStorage;
        Settings = (SpriteCharacterSettings)SettingsManager.Current.Profile.Active.CharacterSetup;
    }

    [RelayCommand]
    public async Task ImportPngtuberPlus()
    {
        var path = await GetStorageProvider().OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Select a Pngtuber Plus save file",
            FileTypeFilter = new[] { FilePickers.PngTuberPlus },
            AllowMultiple = false
        });
        var filePath = path.FirstOrDefault()?.Path?.AbsolutePath;
        if (string.IsNullOrEmpty(filePath)) return;

        var file = await File.ReadAllTextAsync(filePath);
        var obj = JsonSerializer.Deserialize<Dictionary<string, PngTuberPlusObject>>(file);
        var items = obj.Values.ToList();
        var parent = items.First(x => x.parentId == null);
        items.Remove(parent);

        SpriteImage spriteParent = null;
        await Task.Run(async () =>
        {
            // takes some time, maybe show progress
            spriteParent = PngTuberPlusMigrator.MigratePngtuberPlus(parent, items);
        });
        Settings.Parent = spriteParent;

        LayerManager.CharacterStateHandler.CharacterSetup.RefreshCharacterSettings();
    }
}
