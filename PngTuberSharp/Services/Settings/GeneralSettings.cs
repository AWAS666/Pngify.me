using CommunityToolkit.Mvvm.ComponentModel;

namespace PngTuberSharp.Services.Settings
{
    public partial class GeneralSettings : ObservableObject
    {
        [ObservableProperty]
        private bool? enableSpout = false;

    }
}
