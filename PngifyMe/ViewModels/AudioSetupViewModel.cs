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

        public List<string> InputDevices => MicrophoneService.GetAllInDevices();
        public List<string> OutputDevices => MicrophoneService.GetAllOutDevices();

        public AudioSetupViewModel()
        {

        }
    }
}
