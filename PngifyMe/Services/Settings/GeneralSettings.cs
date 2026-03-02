using CommunityToolkit.Mvvm.ComponentModel;

namespace PngifyMe.Services.Settings
{
    public partial class GeneralSettings : ObservableObject
    {
        [ObservableProperty]
        private bool? enableSpout = false;

        [ObservableProperty]
        private bool? enableWebOutput = false;

        /// <summary>Hex color for web output transparent areas (e.g. #00FF00 for green screen). Used with BMP + OBS chroma key.</summary>
        [ObservableProperty]
        private string webOutputBackgroundColor = "#00FF00";
    }
}
