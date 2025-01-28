using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Layers;
using PngifyMe.Services;
using PngifyMe.Services.CharacterSetup.Basic;
using PngifyMe.Services.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;


namespace PngifyMe.ViewModels;

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
        Settings = (BasicCharSettings)SettingsManager.Current.Profile.Active.AvatarSettings;
        baseStates = Settings.States;
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
        LayerManager.CharacterStateHandler.ToggleState(vm.State.Name);
    }

    public void Apply()
    {
        SettingsManager.Save();
        LayerManager.CharacterStateHandler.SetupHotKeys();
    }
}
