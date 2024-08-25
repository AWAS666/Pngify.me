using CommunityToolkit.Mvvm.ComponentModel;

namespace PngTuberSharp.Services.Settings
{
    public partial class TwitchSettings : ObservableObject
    {
        [ObservableProperty]
        private bool? enabled = false;

    }
}
