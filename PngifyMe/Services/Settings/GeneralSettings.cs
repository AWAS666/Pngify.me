using CommunityToolkit.Mvvm.ComponentModel;

namespace PngifyMe.Services.Settings
{
    public partial class GeneralSettings : ObservableObject
    {
        [ObservableProperty]
        private bool? enableSpout = false;

    }
}
