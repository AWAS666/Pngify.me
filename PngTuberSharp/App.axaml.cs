using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using PngTuberSharp.Services;
using PngTuberSharp.Services.Hotkey;
using PngTuberSharp.Services.Twitch;
using PngTuberSharp.ViewModels;
using PngTuberSharp.Views;

namespace PngTuberSharp;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            WinHotkey.Start(desktop);
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };

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
}
