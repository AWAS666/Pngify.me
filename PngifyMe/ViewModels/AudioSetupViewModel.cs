using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Services;
using PngifyMe.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngifyMe.ViewModels
{
    public partial class AudioSetupViewModel : ObservableObject
    {
        public MicroPhoneSettings Settings => SettingsManager.Current.Profile.Active.MicroPhone;

        public List<AudioDeviceConfig> InputDevices => AudioService.InputDevices;
        public List<AudioDeviceConfig> OutputDevices => AudioService.OutputDevices;

        public AudioSetupViewModel()
        {

        }
    }
}
