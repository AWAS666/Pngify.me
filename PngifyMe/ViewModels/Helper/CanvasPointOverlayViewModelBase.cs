using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PngifyMe.Services;
using System;

namespace PngifyMe.ViewModels.Helper;

/// <summary>
/// Base for canvas position overlay view models. Shared by layer position overlay and sprite position/anchor overlay so the same view can be used.
/// Coordinate conversion (letterboxing + logical &lt;-&gt; bitmap) lives here so the view stays thin.
/// </summary>
public abstract partial class CanvasPointOverlayViewModelBase : ObservableObject
{
    private bool _isRefreshing;

    /// <summary>
    /// Offset of the "drawing" origin from the bitmap top-left. Default (0,0). Used by default ToBitmapPosition/FromBitmapPosition.
    /// </summary>
    public virtual float CanvasOriginOffsetX => 0;
    public virtual float CanvasOriginOffsetY => 0;

    [ObservableProperty]
    private float _x;

    [ObservableProperty]
    private float _y;

    protected void BeginPositionRefresh() => _isRefreshing = true;
    protected void EndPositionRefresh() => _isRefreshing = false;

    protected abstract void PushToModel();

    partial void OnXChanged(float value)
    {
        if (!_isRefreshing) PushToModel();
    }

    partial void OnYChanged(float value)
    {
        if (!_isRefreshing) PushToModel();
    }

    [RelayCommand]
    private void Close()
    {
        CanvasOverlayService.ClearOverlay();
    }

    /// <summary>
    /// Convert logical position (VM X,Y) to bitmap pixel position. Override when the renderer uses extra transforms (e.g. sprite translate + global zoom).
    /// </summary>
    public virtual (float bitmapX, float bitmapY) ToBitmapPosition(float logicalX, float logicalY)
    {
        return (logicalX + CanvasOriginOffsetX, logicalY + CanvasOriginOffsetY);
    }

    /// <summary>
    /// Convert bitmap pixel position to logical position (VM X,Y). Override together with ToBitmapPosition.
    /// </summary>
    public virtual (float logicalX, float logicalY) FromBitmapPosition(float bitmapX, float bitmapY)
    {
        return (bitmapX - CanvasOriginOffsetX, bitmapY - CanvasOriginOffsetY);
    }

    /// <summary>
    /// Same letterboxing as SKImageViewer: bitmap is scaled to fit (min scale) and centered.
    /// </summary>
    public static (double scale, double left, double top) GetBitmapRectInControl(double controlWidth, double controlHeight, int bitmapWidth, int bitmapHeight)
    {
        if (bitmapWidth <= 0 || bitmapHeight <= 0) return (0, 0, 0);
        double scale = Math.Min(controlWidth / bitmapWidth, controlHeight / bitmapHeight);
        double w = bitmapWidth * scale;
        double h = bitmapHeight * scale;
        double left = (controlWidth - w) / 2;
        double top = (controlHeight - h) / 2;
        return (scale, left, top);
    }

    /// <summary>
    /// Convert logical (VM) position to control coordinates for placing the handle. Uses letterboxing and ToBitmapPosition.
    /// </summary>
    public (double controlX, double controlY) LogicalToControl(double controlWidth, double controlHeight, int bitmapWidth, int bitmapHeight)
    {
        var (scale, left, top) = GetBitmapRectInControl(controlWidth, controlHeight, bitmapWidth, bitmapHeight);
        if (scale <= 0) return (0, 0);
        var (bx, by) = ToBitmapPosition(X, Y);
        return (left + bx * scale, top + by * scale);
    }

    /// <summary>
    /// Convert control coordinates to logical (VM) position. Uses letterboxing and FromBitmapPosition.
    /// </summary>
    public (float logicalX, float logicalY) ControlToLogical(double controlX, double controlY, double controlWidth, double controlHeight, int bitmapWidth, int bitmapHeight)
    {
        var (scale, left, top) = GetBitmapRectInControl(controlWidth, controlHeight, bitmapWidth, bitmapHeight);
        if (scale <= 0) return (0, 0);
        float bx = (float)((controlX - left) / scale);
        float by = (float)((controlY - top) / scale);
        return FromBitmapPosition(bx, by);
    }
}
