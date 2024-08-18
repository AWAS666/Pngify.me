using CommunityToolkit.Mvvm.ComponentModel;

namespace PngTuberSharp.ViewModels.Helper
{
    public partial class PropertyViewModel : ObservableObject
    {
        [ObservableProperty]
        private string name;      

        [ObservableProperty]
        private string value;

        [ObservableProperty]
        private string unit;
    }
}
