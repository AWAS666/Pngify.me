using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngifyMe.ViewModels
{
    public partial class GeneralSettingsViewModel: ObservableObject
    {
        [ObservableProperty]
        private SpecsSettings specSettings = Specsmanager.Settings;
    }
}
