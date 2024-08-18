using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace PngTuberSharp.Services.Settings
{
    public partial class TwitchSettings : ObservableObject
    {
        [ObservableProperty]
        private bool? enabled = false;

    }
}
