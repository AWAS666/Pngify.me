using Avalonia.Controls;
using Avalonia.Data;
using PngTuberSharp.Services;
using System;

namespace PngTuberSharp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        grid.Background = SettingsManager.Current.Background.Colour;

        var binding = new Binding
        {
            Source = SettingsManager.Current.Background,
            Path = nameof(SettingsManager.Current.Background.Colour)
        };

        grid.Bind(Grid.BackgroundProperty, binding);

    }

    private void DoubleClick(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        settings.IsVisible = !settings.IsVisible;
    }

}
