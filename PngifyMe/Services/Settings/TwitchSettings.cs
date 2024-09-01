using CommunityToolkit.Mvvm.ComponentModel;

namespace PngifyMe.Services.Settings
{
    public partial class TwitchSettings : ObservableObject
    {
        [ObservableProperty]
        private bool? enabled = false;

    }
}
