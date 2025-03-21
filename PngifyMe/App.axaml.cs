using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using PngifyMe.Services;
using PngifyMe.Services.Hotkey;
using PngifyMe.Services.Twitch;
using PngifyMe.Services.WebSocket;
using PngifyMe.Steam;
using PngifyMe.ViewModels;
using PngifyMe.Views;
using Serilog;
using System;
using System.Threading.Tasks;

namespace PngifyMe;

public partial class App : Application
{
    private MainWindow _mainWindow;
    private SplashScreen _splashScreenWindow;
    private SplashScreenViewModel _splashVM;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            // Open splash screen as early as possible
            _splashScreenWindow = new SplashScreen();
            _splashVM = new SplashScreenViewModel();
            _splashScreenWindow.DataContext = _splashVM;
            desktopLifetime.MainWindow = _splashScreenWindow;

            _splashScreenWindow.Show();
            await UpdateSplash("Loading Settings");
            await SettingsManager.LoadAsync();

            await UpdateSplash("Starting app");

            Dispatcher.UIThread.Post(async () => await CompleteApplicationStart(), DispatcherPriority.Background);
            //await CompleteApplicationStart();
        }
        base.OnFrameworkInitializationCompleted();
    }

    public async Task CompleteApplicationStart()
    {
        // load language here, either system or steam
        Lang.Resources.Culture = SteamLocalization.GetStartUpCulture();
        await UpdateSplash("Starting audio");
        AudioService.Init();
        await UpdateSplash("Starting Websocket Server");
        WebSocketServer.Start();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _mainWindow = new MainWindow();
            HotkeyManager.Start(desktop);

            if (SettingsManager.Current.Twitch.Enabled == true)
            {
                await UpdateSplash("Connecting to twitch");
                await TwitchEventSocket.Start();
            }

            if (OperatingSystem.IsWindows())
            {
                await UpdateSplash("Init Spout");
                SpoutRenderer.Init();
            }

            desktop.MainWindow = _mainWindow;
            await UpdateSplash("Finishing");
            _mainWindow.Show();

            // close splashscreen
            _splashScreenWindow.Close();
        }
    }

    public async Task UpdateSplash(string text)
    {
        _splashVM.Text = text;
        await Task.Delay(1); // needed for ui to update
    }

    private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        Log.Fatal($"Task error: {e.Exception.Message}");
    }
}
