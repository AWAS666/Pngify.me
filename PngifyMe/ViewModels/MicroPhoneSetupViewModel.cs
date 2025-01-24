using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Layers;
using PngifyMe.Services;
using PngifyMe.Services.CharacterSetup;
using PngifyMe.Services.CharacterSetup.Basic;
using PngifyMe.Services.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;


namespace PngifyMe.ViewModels
{
    public partial class MicroPhoneSetupViewModel : ObservableObject
    {
        public Func<IStorageProvider> GetStorageProvider { get; }

        private List<CharacterState> baseStates;

        public BasicCharSettings Settings { get; }
        public MicSettings MicSettings { get; }

        [ObservableProperty]
        private ObservableCollection<MicroPhoneStateViewModel> states;

        public MicroPhoneSetupViewModel() : this(null)
        {

        }

        public MicroPhoneSetupViewModel(Func<IStorageProvider> getStorage)
        {
            GetStorageProvider = getStorage;
            baseStates = SettingsManager.Current.Profile.Active.CharacterSetup.States;
            Settings = SettingsManager.Current.Profile.Active.CharacterSetup;
            MicSettings = SettingsManager.Current.Profile.Active.MicSettings;
            states = new ObservableCollection<MicroPhoneStateViewModel>(baseStates.Select(x => new MicroPhoneStateViewModel(x, this)));
        }

        public void Add()
        {
            var set = new CharacterState();
            States.Add(new MicroPhoneStateViewModel(set, this));
            baseStates.Add(set);
        }

        public void Remove(MicroPhoneStateViewModel vm)
        {
            // dont allow removal of final state
            if (States.Count <= 1)
                return;
            States.Remove(vm);
            baseStates.Remove(vm.State);
        }

        public void SwitchState(MicroPhoneStateViewModel vm)
        {
            LayerManager.CharacterStateHandler.ToggleState(vm.State);
        }

        public void Apply()
        {
            SettingsManager.Save();
            LayerManager.CharacterStateHandler.SetupHotKeys();
        }
    }
}
