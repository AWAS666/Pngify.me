using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Services;

namespace PngifyMe.ViewModels
{
    public partial class GeneralSettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private SpecsSettings specSettings = Specsmanager.Settings;
    }
}
