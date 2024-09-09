using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using PngifyMe.Services;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PngifyMe.Views;

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

        this.Title = $"{Title}-{Assembly.GetExecutingAssembly().GetName().Version?.ToString()}-beta";
    }

    private void DoubleClick(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        settings.IsVisible = !settings.IsVisible;
    }

    private void SaveSettings(object? sender, RoutedEventArgs e)
    {
        SettingsManager.Save();
        Specsmanager.Save();
    }  

}
