using System;

namespace PngifyMe.Services;

/// <summary>
/// Static service for showing overlay content on top of the avatar view.
/// </summary>
public static class CanvasOverlayService
{
    private static object? _currentOverlayViewModel;

    public static object? CurrentOverlayViewModel => _currentOverlayViewModel;

    /// <summary>
    /// Raised when CurrentOverlayViewModel changes. Subscribe in MainWindow to update overlay ContentControl.
    /// </summary>
    public static event EventHandler<object?>? CurrentOverlayViewModelChanged;

    public static void SetOverlay(object? vm)
    {
        if (_currentOverlayViewModel == vm)
            return;
        _currentOverlayViewModel = vm;
        CurrentOverlayViewModelChanged?.Invoke(null, vm);
    }

    public static void ClearOverlay()
    {
        SetOverlay(null);
    }
}
