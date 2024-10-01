using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Services;
using PngifyMe.Settings;

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
