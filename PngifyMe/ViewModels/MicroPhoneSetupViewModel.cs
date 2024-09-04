using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Layers;
using PngifyMe.Services;
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

        private List<MicroPhoneState> baseStates;

        public MicroPhoneSettings Settings { get; }

        [ObservableProperty]
        private ObservableCollection<MicroPhoneStateViewModel> states;

        public MicroPhoneSetupViewModel() : this(null)
        {

        }

        public MicroPhoneSetupViewModel(Func<IStorageProvider> getStorage)
        {
            GetStorageProvider = getStorage;
            baseStates = SettingsManager.Current.Profile.Active.MicroPhone.States;
            Settings = SettingsManager.Current.Profile.Active.MicroPhone;
            states = new ObservableCollection<MicroPhoneStateViewModel>(baseStates.Select(x => new MicroPhoneStateViewModel(x, this)));
        }

        public void Add()
        {
            var set = new MicroPhoneState();
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
            LayerManager.MicroPhoneStateLayer.ToggleState(vm.State);
        }

        public void Apply()
        {
            SettingsManager.Save();
            LayerManager.MicroPhoneStateLayer.SetupHotKeys();
        }
    }
}
