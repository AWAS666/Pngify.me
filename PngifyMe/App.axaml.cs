using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CppSharp.Types.Std;
using PngifyMe.Helpers;
using PngifyMe.Services;
using PngifyMe.Services.Hotkey;
using PngifyMe.Services.Twitch;
using PngifyMe.Services.WebSocket;
using PngifyMe.Views;
using Serilog;
using Serilog.Events;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace PngifyMe;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        // load language here, either system or steam
        Lang.Resources.Culture = new CultureInfo("en-us");
        AudioService.Init();
        WebSocketServer.Start();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            HotkeyManager.Start(desktop);
            desktop.MainWindow = new MainWindow
            {
            };
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            if (SettingsManager.Current.Twitch.Enabled == true)
                await TwitchEventSocket.Start();

            if (OperatingSystem.IsWindows())
            {
                SpoutRenderer.Init();
            }

        }
        //else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        //{
        //    singleViewPlatform.MainView = new MainView
        //    {
        //        DataContext = new MainViewModel()
        //    };
        //}

        base.OnFrameworkInitializationCompleted();
    }

    private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        Log.Fatal($"Task error: {e.Exception.Message}");
    }    
}
