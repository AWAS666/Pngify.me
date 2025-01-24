using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Services;
using PngifyMe.Services.CharacterSetup;
using PngifyMe.Services.CharacterSetup.Basic;
using PngifyMe.Services.Settings;
using System.Collections.Generic;

namespace PngifyMe.ViewModels
{
    public partial class AudioSetupViewModel : ObservableObject
    {
        public MicSettings Settings => SettingsManager.Current.Profile.Active.MicSettings;

        public List<AudioDeviceConfig> InputDevices => AudioService.InputDevices;
        public List<AudioDeviceConfig> OutputDevices => AudioService.OutputDevices;

        public AudioSetupViewModel()
        {

        }
    }
}
