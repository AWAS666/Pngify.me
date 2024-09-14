using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using PngifyMe.Services;
using PngifyMe.Services.Hotkey;
using PngifyMe.Services.Twitch;
using PngifyMe.ViewModels;
using PngifyMe.Views;
using Serilog;
using System;
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
        SetupSerilog();


        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            WinHotkey.Start(desktop);
            desktop.MainWindow = new MainWindow
            {
            };
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            if (SettingsManager.Current.Twitch.Enabled == true)
                await TwitchEventSocket.Start();

            SpoutRenderer.Init();

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

    private static void SetupSerilog()
    {
        // Define the path to the log file in %localappdata%/appname
        var localAppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PngifyMe", "log-.txt");
        // Ensure the directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(localAppDataPath));
        // Configure Serilog to write to a file
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(localAppDataPath, rollingInterval: RollingInterval.Day)
            .MinimumLevel.Warning()
            .CreateLogger();
    }
}
