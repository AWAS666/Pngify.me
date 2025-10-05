using Avalonia;
using Avalonia.ReactiveUI;
using PngifyMe.Helpers;
using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace PngifyMe.Desktop;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            SetupSerilog();
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            Log.Fatal(e, $"Something very bad happened: {e.Message}");
            Log.Fatal(e, "Something very bad happened: {Message}, Stack Trace: {@StackTrace}", e.Message, e.StackTrace);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {      
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
    }

    private static void SetupSerilog()
    {
        // Define the path to the log file in %localappdata%/appname
        var localAppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PngifyMe", "logs", "log-.txt");
        // Ensure the directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(localAppDataPath));
        // Configure Serilog to write to a file
        //https://github.com/serilog/serilog/wiki/configuration-basics
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(localAppDataPath,
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: LogEventLevel.Debug)
            .WriteTo.Sink(ErrorForwarder.Sink, restrictedToMinimumLevel: LogEventLevel.Information)
            .MinimumLevel.Debug()
            .CreateLogger();
    }
}
