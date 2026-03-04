using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using PngifyMe.Lang;
using PngifyMe.Services;
using PngifyMe.Services.Twitch;

namespace PngifyMe.Views;

public partial class SettingsPanelView : UserControl
{
    public bool IsDetached { get; private set; }

    public event EventHandler? DetachRequested;
    public event EventHandler? AttachRequested;
    public event EventHandler? CloseRequested;
    public event EventHandler? SaveRequested;

    public SettingsPanelView()
    {
        InitializeComponent();
        TwitchEventSocket.Authenticated += OnTwitchAuthenticated;
        if (SettingsManager.Current.Twitch.Enabled == true)
            twitchStatus.Text = PngifyMe.Lang.Resources.Connecting;
    }

    protected override void OnDetachedFromVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        TwitchEventSocket.Authenticated -= OnTwitchAuthenticated;
        base.OnDetachedFromVisualTree(e);
    }

    public void SetDetached(bool detached)
    {
        IsDetached = detached;
        detachDockButtonText.Text = detached ? PngifyMe.Lang.Resources.DockSettings : PngifyMe.Lang.Resources.DetachSettings;
        ToolTip.SetTip(detachDockButton, detached ? PngifyMe.Lang.Resources.ToolTipDockSettings : PngifyMe.Lang.Resources.ToolTipDetachSettings);
        detachDockIcon.IsVisible = !detached;
        dockIcon.IsVisible = detached;
    }

    private void OnDetachDockClick(object? sender, RoutedEventArgs e)
    {
        if (IsDetached)
            AttachRequested?.Invoke(this, e);
        else
            DetachRequested?.Invoke(this, e);
    }

    private void OnSaveClick(object? sender, RoutedEventArgs e) => SaveRequested?.Invoke(this, e);

    private void OnCloseClick(object? sender, RoutedEventArgs e) => CloseRequested?.Invoke(this, e);

    private void OnTwitchPressed(object? sender, PointerPressedEventArgs e) => tabs.SelectedIndex = 7;

    private void OnTwitchAuthenticated(object? sender, TwitchAuth _)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() => twitchStatus.Text = PngifyMe.Lang.Resources.Connected);
    }
}
