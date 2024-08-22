using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using PngTuberSharp.Services;
using PngTuberSharp.Services.Hotkey;
using PngTuberSharp.Services.Twitch;
using PngTuberSharp.ViewModels;
using PngTuberSharp.Views;
using Serilog;
using System.IO;
using System;
using System.Threading.Tasks;

namespace PngTuberSharp;

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
                DataContext = new MainViewModel()
            };
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            if (SettingsManager.Current.Twitch.Enabled == true)
                await TwitchEventSocket.Start();

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
        var localAppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PngTuberSharp", "log-.txt");
        // Ensure the directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(localAppDataPath));
        // Configure Serilog to write to a file
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(localAppDataPath, rollingInterval: RollingInterval.Day)
            .MinimumLevel.Warning()
            .CreateLogger();
    }
}
