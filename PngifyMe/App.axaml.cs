using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using PngifyMe.Layers;
using PngifyMe.Services;
using PngifyMe.Services.Hotkey;
using PngifyMe.Services.Twitch;
using PngifyMe.Services.WebSocket;
using PngifyMe.Steam;
using PngifyMe.ViewModels;
using PngifyMe.Views;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
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
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            // Open splash screen as early as possible
            _splashScreenWindow = new SplashScreen();
            _splashVM = new SplashScreenViewModel();
            _splashScreenWindow.DataContext = _splashVM;
            desktopLifetime.MainWindow = _splashScreenWindow;

            var watch = Stopwatch.StartNew();

            _splashScreenWindow.Show();
            await UpdateSplash("Loading Settings");
            await SettingsManager.LoadAsync();
            await UpdateSplash("Starting app");
            Log.Debug($"Loaded settings in {watch.ElapsedMilliseconds}ms");

            Dispatcher.UIThread.Post(async () => await CompleteApplicationStart(), DispatcherPriority.Background);
            //await CompleteApplicationStart();
        }
        base.OnFrameworkInitializationCompleted();
    }      

    public async Task CompleteApplicationStart()
    {
        try
        {
            var watch = Stopwatch.StartNew();
            // load language here, either system or steam
            Lang.Resources.Culture = SteamLocalization.GetStartUpCulture();
            await UpdateSplash("Starting audio");
            AudioService.Init();
            Log.Debug($"Audio started in {watch.ElapsedMilliseconds}ms");

            watch.Restart();
            await UpdateSplash("Starting Websocket Server");
            WebSocketServer.Start();
            Log.Debug($"Websocket started in {watch.ElapsedMilliseconds}ms");

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                watch.Restart();
                _mainWindow = new MainWindow();
                HotkeyManager.Start(desktop);
                Log.Debug($"HotkeyManager started in {watch.ElapsedMilliseconds}ms");

                if (SettingsManager.Current.Twitch.Enabled == true)
                {
                    watch.Restart();
                    await UpdateSplash("Connecting to twitch");
                    await TwitchEventSocket.Start();
                    Log.Debug($"Twitch started in {watch.ElapsedMilliseconds}ms");
                }

                if (OperatingSystem.IsWindows())
                {
                    watch.Restart();
                    await UpdateSplash("Init Spout");
                    SpoutRenderer.Init();
                    Log.Debug($"Spout started in {watch.ElapsedMilliseconds}ms");
                }

                LayerManager.Start();

                watch.Restart();
                desktop.MainWindow = _mainWindow;
                await UpdateSplash("Finishing");
                _mainWindow.Show();

                // close splashscreen
                _splashScreenWindow.Close();
                Log.Debug($"Finishing Splash in {watch.ElapsedMilliseconds}ms");
            }
        }
        catch (Exception e)
        {
            Log.Error($"App Init Error: {e.Message}", e);
        }
    }

    public async Task UpdateSplash(string text)
    {
        _splashVM.Text = text;
        await Task.Delay(1); // needed for ui to update
    }
}
