using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Services;
using PngifyMe.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngifyMe.ViewModels
{
    public partial class LayoutViewModel : ObservableObject
    {
        public BackgroundSettings Settings { get; }

        public LayoutViewModel()
        {
            Settings = SettingsManager.Current.Background;
        }

    }
}
