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

        public List<string> InputDevices => AudioService.GetAllInDevices();
        public List<string> OutputDevices => AudioService.GetAllOutDevices();

        public AudioSetupViewModel()
        {

        }
    }
}
