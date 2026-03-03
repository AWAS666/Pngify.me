using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using System;

namespace PngifyMe.Services;

/// <summary>
/// Static service for showing overlay content on top of the avatar view.
/// When a source control is provided, the overlay is cleared when that control is detached from the visual tree.
/// </summary>
public static class CanvasOverlayService
{
    private static object? _currentOverlayViewModel;
    private static Control? _sourceControl;
    private static EventHandler<VisualTreeAttachmentEventArgs>? _detachHandler;

    public static object? CurrentOverlayViewModel => _currentOverlayViewModel;

    /// <summary>
    /// Raised when CurrentOverlayViewModel changes. Subscribe in MainWindow to update overlay ContentControl.
    /// </summary>
    public static event EventHandler<object?>? CurrentOverlayViewModelChanged;

    public static void SetOverlay(object? vm, Control? sourceControl = null)
    {
        UnsubscribeFromSource();
        _sourceControl = null;

        _currentOverlayViewModel = vm;
        CurrentOverlayViewModelChanged?.Invoke(null, vm);

        if (vm != null && sourceControl != null)
        {
            _sourceControl = sourceControl;
            _detachHandler = (_, _) => ClearOverlay();
            _sourceControl.DetachedFromVisualTree += _detachHandler;
        }
    }

    public static void ClearOverlay()
    {
        UnsubscribeFromSource();
        _sourceControl = null;
        _currentOverlayViewModel = null;
        CurrentOverlayViewModelChanged?.Invoke(null, null);
    }

    private static void UnsubscribeFromSource()
    {
        if (_sourceControl != null && _detachHandler != null)
        {
            _sourceControl.DetachedFromVisualTree -= _detachHandler;
            _detachHandler = null;
        }
    }
}
