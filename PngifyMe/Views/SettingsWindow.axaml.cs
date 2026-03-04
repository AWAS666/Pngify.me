using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace PngifyMe.Views;

public partial class SettingsWindow : Window
{
    private Action? _onClosingCallback;

    public SettingsWindow()
    {
        InitializeComponent();
    }

    public void SetClosingCallback(Action? callback)
    {
        _onClosingCallback = callback;
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        var callback = _onClosingCallback;
        _onClosingCallback = null;
        callback?.Invoke();
        base.OnClosing(e);
    }
}
