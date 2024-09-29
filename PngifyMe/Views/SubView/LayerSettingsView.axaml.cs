using Avalonia.Controls;
using Avalonia.Interactivity;
using PngifyMe.ViewModels;
using PngifyMe.ViewModels.Helper;
using Ursa.Controls;

namespace PngifyMe.Views.Helper;

public partial class LayerSettingsView : UserControl
{
    public LayerSettingsView()
    {
        InitializeComponent();
    }

    private async void ShowSelector(object sender, RoutedEventArgs e)
    {
        var parent = (LayersettingViewModel)DataContext;

        var vm = new LayerSelectorViewModel(parent);

        var options = new OverlayDialogOptions()
        {
            Title = "Layer Selector",
            CanLightDismiss = false,
            CanDragMove = true,
            IsCloseButtonVisible = false,
            HorizontalAnchor = HorizontalPosition.Center,
            VerticalAnchor = VerticalPosition.Center,
            Mode = DialogMode.None,
        };
        await OverlayDialog.ShowCustomModal<LayerSelectorView, LayerSelectorViewModel, object>(vm, null, options: options);
    }
}