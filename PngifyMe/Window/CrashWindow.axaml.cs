using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Diagnostics;
using System.IO;

namespace PngifyMe;

public partial class CrashWindow : Window
{
    private const string SupportUrl = "https://discord.gg/ZeyD2ZgHTS";
    private readonly Exception? _exception;
    private readonly string _logsFolderPath;

    public CrashWindow(Exception? exception = null)
    {
        _exception = exception;
        _logsFolderPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
            "PngifyMe", 
            "logs"
        );
        
        InitializeComponent();
        DataContext = this;
        
        DisplayException();
    }

    public string SupportUrlValue => SupportUrl;

    private void DisplayException()
    {
        if (_exception == null)
        {
            ExceptionMessageText.Text = "An unknown error occurred.";
            StackTraceText.Text = "No stack trace available.";
            return;
        }

        ExceptionMessageText.Text = $"{_exception.GetType().Name}: {_exception.Message}";

        if (_exception.InnerException != null)
        {
            ExceptionMessageText.Text += $"\n\nInner Exception: {_exception.InnerException.GetType().Name}: {_exception.InnerException.Message}";
        }

        StackTraceText.Text = _exception.StackTrace ?? "No stack trace available.";
    }

    private void OpenLogsButton_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (Directory.Exists(_logsFolderPath))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = _logsFolderPath,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            else
            {
                var messageWindow = new Window
                {
                    Title = "Logs Folder Not Found",
                    Width = 400,
                    Height = 150,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Content = new TextBlock
                    {
                        Text = $"The logs folder does not exist yet:\n{_logsFolderPath}",
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Thickness(20)
                    }
                };
                messageWindow.ShowDialog(this);
            }
        }
        catch (Exception ex)
        {
            var messageWindow = new Window
            {
                Title = "Error",
                Width = 400,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Content = new TextBlock
                {
                    Text = $"Failed to open logs folder:\n{ex.Message}",
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(20)
                }
            };
            messageWindow.ShowDialog(this);
        }
    }

    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void SupportLink_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = SupportUrl,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            var messageWindow = new Window
            {
                Title = "Error",
                Width = 400,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Content = new TextBlock
                {
                    Text = $"Failed to open support URL:\n{ex.Message}",
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(20)
                }
            };
            messageWindow.ShowDialog(this);
        }
    }
}