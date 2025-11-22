using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using PngifyMe.Helpers;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

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

            if (args.Length > 0 && args[0] == "--crash")
            {
                ShowCrashWindowMode();
                return;
            }

            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Something very bad happened: {Message}, Stack Trace: {@StackTrace}", e.Message, e.StackTrace);
            RestartWithCrashWindow(e);
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
            .WriteTo.Sink(LogViewerForwarder.Sink, restrictedToMinimumLevel: LogEventLevel.Debug)
            .MinimumLevel.Debug()
            .CreateLogger();

        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        AppDomain.CurrentDomain.UnhandledException += UnhandledExcpection;
    }

    private static void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        Log.Fatal($"Task error: {e.Exception.Message}");
        e.SetObserved();
        RestartWithCrashWindow(e.Exception);
    }
    private static void UnhandledExcpection(object s, UnhandledExceptionEventArgs e)
    {
        var ex = e.ExceptionObject as Exception;
        Log.Error(ex, $"[Global] {ex}");
        RestartWithCrashWindow(ex);
    }

    private static void RestartWithCrashWindow(Exception? ex)
    {
        try
        {
            // Save exception details to a temporary file as JSON
            var crashFilePath = Path.Combine(Path.GetTempPath(), "PngifyMe_crash.json");
            var crashData = CrashLogData.FromException(ex);

            var json = JsonSerializer.Serialize(crashData, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(crashFilePath, json);

            // Restart the process with crash flag
            var currentProcess = Process.GetCurrentProcess();
            var processPath = currentProcess.MainModule?.FileName ?? Environment.ProcessPath;

            if (!string.IsNullOrEmpty(processPath))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = processPath,
                    Arguments = "--crash",
                    UseShellExecute = true
                });
            }
        }
        catch (Exception e)
        {
            Log.Error(e, "Failed to restart with crash window: " + e.Message);
        }

        Environment.Exit(1);
    }

    private static void ShowCrashWindowMode()
    {
        try
        {
            // Read exception details from temp file
            var crashFilePath = Path.Combine(Path.GetTempPath(), "PngifyMe_crash.json");
            Exception? exception = null;

            if (File.Exists(crashFilePath))
            {
                var json = File.ReadAllText(crashFilePath);
                var crashData = JsonSerializer.Deserialize<CrashLogData>(json);

                if (crashData != null)
                {
                    exception = crashData.ToException();
                }

                // Clean up crash file
                try { File.Delete(crashFilePath); } catch { }
            }

            // Show crash window in a clean minimal application instance
            var lifetime = new ClassicDesktopStyleApplicationLifetime
            {
                ShutdownMode = ShutdownMode.OnMainWindowClose
            };

            AppBuilder.Configure<CrashApp>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace()
                .SetupWithLifetime(lifetime);

            var crashWindow = new CrashWindow(exception);
            lifetime.MainWindow = crashWindow;

            lifetime.Start(new string[] { });
        }
        catch (Exception e)
        {
            Log.Error(e, $"CRITICAL ERROR - Failed to show crash window: {e.Message}");
        }
    }
}
