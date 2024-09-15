using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using PngifyMe.Services;
using PngifyMe.Services.Twitch;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using TwitchLib.Api.Core.HttpCallHandlers;

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
        TwitchEventSocket.Authenticated += Authenticated;
        if (SettingsManager.Current.Twitch.Enabled == true)
            twitchStatus.Text = "Connecting";
    }

    private void Authenticated(object? sender, TwitchAuth e)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            twitchStatus.Text = "Connected";
        });
    }

    private void DoubleClick(object? sender, TappedEventArgs e)
    {
        settings.IsVisible = !settings.IsVisible;
        hintBox.IsVisible = false;
    }

    private void HintPressed(object? sender, PointerPressedEventArgs e)
    {
        hintBox.IsVisible = false;
    }

    private void TwitchPressed(object? sender, PointerPressedEventArgs e)
    {
        tabs.SelectedIndex = 7;
    }   

    private void SaveSettings(object? sender, RoutedEventArgs e)
    {
        SettingsManager.Save();
        Specsmanager.Save();
    }

}
