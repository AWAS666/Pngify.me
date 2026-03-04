using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using PngifyMe.Helpers;
using PngifyMe.Services;
using PngifyMe.ViewModels.Helper;
using PngifyMe.Views.Helper;
using Serilog;
using System.Linq;
using System.Reflection;
using Ursa.Controls;

namespace PngifyMe.Views;

public partial class MainWindow : Window
{
    private SettingsPanelView? _settingsPanelView;
    private SettingsWindow? _settingsWindow;

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

        this.Title = $"{Title}-{Assembly.GetExecutingAssembly().GetName().Version?.ToString()}";

        var topLevel = TopLevel.GetTopLevel(this);
        ErrorForwarder.Sink.SetNotificationHandler(new WindowNotificationManager(topLevel) { MaxItems = 3 });
        Log.Information("Double Click your avatar to hide the settings");

        _settingsPanelView = new SettingsPanelView();
        _settingsPanelView.DetachRequested += OnSettingsDetachRequested;
        _settingsPanelView.AttachRequested += OnSettingsAttachRequested;
        _settingsPanelView.CloseRequested += OnSettingsCloseRequested;
        _settingsPanelView.SaveRequested += OnSettingsSaveRequested;
        settingsContentHost.Content = _settingsPanelView;

        CanvasOverlayService.CurrentOverlayViewModelChanged += OnCanvasOverlayViewModelChanged;
#if DEBUG
        CanvasOverlayService.SetOverlay(new TestOverlayViewModel());
#endif
    }

    private void OnCanvasOverlayViewModelChanged(object? sender, object? vm)
    {
        Dispatcher.UIThread.Post(() =>
        {
            canvasOverlayHost.Content = vm;
            canvasOverlayHost.IsHitTestVisible = vm != null;
        });
    }


    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (_settingsPanelView?.IsDetached == true)
            Reattach();
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

    private void DoubleClick(object? sender, TappedEventArgs e)
    {
        if (_settingsPanelView?.IsDetached == true)
        {
            _settingsWindow?.Activate();
            return;
        }
        settings.IsVisible = !settings.IsVisible;
        ErrorForwarder.Sink.SetActive(settings.IsVisible);
        if (!settings.IsVisible)
            CanvasOverlayService.ClearOverlay();
    }

    private void EscDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
            DoubleClick(null, null);
    }

    private void OnSettingsDetachRequested(object? sender, EventArgs e)
    {
        if (_settingsPanelView == null) return;
        settings.IsVisible = false;
        ErrorForwarder.Sink.SetActive(false);
        settingsContentHost.Content = null;
        _settingsWindow ??= new SettingsWindow();
        _settingsWindow.SetClosingCallback(Reattach);
        _settingsWindow.Content = _settingsPanelView;
        _settingsPanelView.SetDetached(true);
        _settingsWindow.Show(this);
    }

    private void OnSettingsAttachRequested(object? sender, EventArgs e) => Reattach();

    private void OnSettingsCloseRequested(object? sender, EventArgs e)
    {
        if (_settingsPanelView?.IsDetached == true)
        {
            Reattach();
            settings.IsVisible = false;
            ErrorForwarder.Sink.SetActive(false);
            CanvasOverlayService.ClearOverlay();
        }
        else
        {
            settings.IsVisible = false;
            ErrorForwarder.Sink.SetActive(false);
            CanvasOverlayService.ClearOverlay();
        }
    }

    private void OnSettingsSaveRequested(object? sender, EventArgs e)
    {
        SettingsManager.Save();
        Specsmanager.Save();
        CanvasOverlayService.ClearOverlay();
    }

    private void Reattach()
    {
        if (_settingsPanelView == null) return;
        if (_settingsWindow != null)
        {
            _settingsWindow.SetClosingCallback(null);
            _settingsWindow.Content = null;
            _settingsWindow.Close();
            _settingsWindow = null;
        }
        settingsContentHost.Content = _settingsPanelView;
        _settingsPanelView.SetDetached(false);
        settings.IsVisible = true;
        ErrorForwarder.Sink.SetActive(true);
    }
}
