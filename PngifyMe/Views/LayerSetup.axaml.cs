using Avalonia.Controls;
using PngifyMe.Services;
using PngifyMe.ViewModels;

namespace PngifyMe.Views;

public partial class LayerSetup : UserControl
{
    public LayerSetup()
    {
        InitializeComponent();
        DataContext = new LayerSetupViewModel(SettingsManager.Current.LayerSetup.Layers);
    }



}