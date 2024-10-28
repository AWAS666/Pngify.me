using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using PngifyMe.Helpers;
using PngifyMe.Services;
using PngifyMe.Services.Twitch;
using PngifyMe.Views.Helper;
using Serilog;
using System.Linq;
using System.Reflection;
using Ursa.Controls;

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

        var topLevel = TopLevel.GetTopLevel(this);
        ErrorForwarder.Sink.SetNotificationHandler(new WindowNotificationManager(topLevel) { MaxItems = 3 });
        Log.Information("Double Click your avatar to hide the settings");
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        ScreenPosition.Save(new ScreenPosition()
        {
            Left = Position.X,
            Top = Position.Y,
            Width = Width,
            Height = Height,
            WindowState = WindowState
        });
        base.OnClosing(e);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        // somehow this offset a little sometimes??
        var pos = ScreenPosition.Load();
        if (pos != null)
        {
            switch (pos.WindowState)
            {
                case WindowState.Normal:
                    var newPos = new PixelPoint(pos.Left, pos.Top);
                    // check if on screen 
                    if (Screens.All.FirstOrDefault(x => x.WorkingArea.Contains(newPos)) == null)
                        return;
                    Width = pos.Width;
                    Height = pos.Height;
                    break;
                case WindowState.Minimized:
                    break;
                case WindowState.Maximized:
                    break;
                case WindowState.FullScreen:
                    break;
                default:
                    break;
            }
            WindowState = pos.WindowState;
        }
        base.OnLoaded(e);
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
        ErrorForwarder.Sink.SetActive(settings.IsVisible);
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

    private void CloseSettings(object? sender, RoutedEventArgs e)
    {
        settings.IsVisible = false;
        ErrorForwarder.Sink.SetActive(false);
    }

}
