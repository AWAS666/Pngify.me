using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using PngifyMe.Services;
using PngifyMe.Settings;
using PngifyMe.ViewModels;
using System;
using System.Linq;
using System.Reflection;

namespace PngifyMe.Views;

public partial class LayoutMenu : UserControl
{
    public LayoutMenu()
    {
        InitializeComponent();
        //background.ItemsSource = typeof(Brushes)
        //    .GetProperties(BindingFlags.Public | BindingFlags.Static)
        //     .Where(p => typeof(IBrush).IsAssignableFrom(p.PropertyType))
        //    .Select(p => (IBrush)p.GetValue(null));

        DataContext = new LayoutViewModel();
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