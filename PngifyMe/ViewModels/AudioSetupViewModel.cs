using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PngifyMe.Services;
using PngifyMe.Services.CharacterSetup;
using PngifyMe.Services.CharacterSetup.Basic;
using PngifyMe.Services.Settings;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PngifyMe.ViewModels
{
    public partial class AudioSetupViewModel : ObservableObject
    {
        public MicSettings Settings => SettingsManager.Current.Profile.Active.MicSettings;

        public ObservableCollection<AudioDeviceConfig> InputDevices { get; private set; } = new ObservableCollection<AudioDeviceConfig>(AudioService.InputDevices);
        public ObservableCollection<AudioDeviceConfig> OutputDevices { get; private set; } = new ObservableCollection<AudioDeviceConfig>(AudioService.OutputDevices);

        public AudioSetupViewModel()
        {

        }      
    }
}
