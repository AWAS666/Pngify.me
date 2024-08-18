using Avalonia.Controls;
using PngTuberSharp.Services;
using PngTuberSharp.ViewModels;

namespace PngTuberSharp.Views;

public partial class LayerSetup : UserControl
{
    public LayerSetup()
    {
        InitializeComponent();
        DataContext = new LayerSetupViewModel(SettingsManager.Current.LayerSetup.Layers);
    }
}