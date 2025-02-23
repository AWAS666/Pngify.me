using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using PngifyMe.Settings;
using PngifyMe.ViewModels;

namespace PngifyMe.Views;

public partial class LayoutMenu : UserControl
{
    public LayoutMenu()
    {
        InitializeComponent();

        var vm = new LayoutViewModel();
        DataContext = vm;
        if (vm.Settings.Colour is ImmutableSolidColorBrush solidColorBrush)
        {
            var color = solidColorBrush.Color;
            colorPicker.Color = new Color(color.A, color.R,color.G, color.B);
        }
    }

    private void ColorChanged(object sender, ColorChangedEventArgs e)
    {
        var vm = (LayoutViewModel)DataContext;
        var color = e.NewColor;
        vm.Settings.Colour = BackgroundSettings.HexToBrush($"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}");
    }



    private void SetToTransparant(object sender, RoutedEventArgs e)
    {
        var vm = (LayoutViewModel)DataContext;
        vm.Settings.Colour = Brushes.Transparent;
        colorPicker.Color = new Color(0, colorPicker.Color.R, colorPicker.Color.G, colorPicker.Color.B);
    }
}