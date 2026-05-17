using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PngifyMe.Services;
using System;

namespace PngifyMe.ViewModels.Helper;

/// <summary>
/// Base for canvas region overlay view models. X/Y are center offsets from the canvas center; Width/Height are the box size in pixels.
/// </summary>
public abstract partial class CanvasRegionOverlayViewModelBase : ObservableObject
{
    private bool _isRefreshing;

    [ObservableProperty]
    private float _x;

    [ObservableProperty]
    private float _y;

    [ObservableProperty]
    private float _width;

    [ObservableProperty]
    private float _height;

    protected bool IsRefreshing => _isRefreshing;
    protected void BeginRefresh() => _isRefreshing = true;
    protected void EndRefresh() => _isRefreshing = false;

    protected abstract void PushToModel();

    partial void OnXChanged(float value)
    {
        if (!_isRefreshing) PushToModel();
    }

    partial void OnYChanged(float value)
    {
        if (!_isRefreshing) PushToModel();
    }

    partial void OnWidthChanged(float value)
    {
        if (!_isRefreshing && _pushSizeChangesToModel) PushToModel();
    }

    partial void OnHeightChanged(float value)
    {
        if (!_isRefreshing && _pushSizeChangesToModel) PushToModel();
    }

    private bool _pushSizeChangesToModel = true;

    public void SetPushSizeChangesToModel(bool value) => _pushSizeChangesToModel = value;

    public void CommitToModel() => PushToModel();

    [RelayCommand]
    private void Close()
    {
        CanvasOverlayService.ClearOverlay();
    }

    public (float bitmapX, float bitmapY) CenterToBitmap(float centerOffsetX, float centerOffsetY)
    {
        return (Specsmanager.Width / 2f + centerOffsetX, Specsmanager.Height / 2f + centerOffsetY);
    }

    public (float centerOffsetX, float centerOffsetY) BitmapToCenter(float bitmapX, float bitmapY)
    {
        return (bitmapX - Specsmanager.Width / 2f, bitmapY - Specsmanager.Height / 2f);
    }

    public (double left, double top, double controlWidth, double controlHeight) RectToControl(
        double hostWidth, double hostHeight, int bitmapWidth, int bitmapHeight)
    {
        var (scale, left, top) = CanvasPointOverlayViewModelBase.GetBitmapRectInControl(hostWidth, hostHeight, bitmapWidth, bitmapHeight);
        if (scale <= 0) return (0, 0, 0, 0);

        var (cx, cy) = CenterToBitmap(X, Y);
        double controlLeft = left + (cx - Width / 2f) * scale;
        double controlTop = top + (cy - Height / 2f) * scale;
        return (controlLeft, controlTop, Width * scale, Height * scale);
    }

    public void ControlRectToLogical(double controlLeft, double controlTop, double controlW, double controlH,
        double hostWidth, double hostHeight, int bitmapWidth, int bitmapHeight)
    {
        var (scale, bitmapLeft, bitmapTop) = CanvasPointOverlayViewModelBase.GetBitmapRectInControl(hostWidth, hostHeight, bitmapWidth, bitmapHeight);
        if (scale <= 0) return;

        float bitmapW = (float)(controlW / scale);
        float bitmapH = (float)(controlH / scale);
        float bitmapCx = (float)((controlLeft - bitmapLeft) / scale + bitmapW / 2f);
        float bitmapCy = (float)((controlTop - bitmapTop) / scale + bitmapH / 2f);
        var (offsetX, offsetY) = BitmapToCenter(bitmapCx, bitmapCy);

        BeginRefresh();
        try
        {
            X = offsetX;
            Y = offsetY;
            Width = Math.Max(1, bitmapW);
            Height = Math.Max(1, bitmapH);
        }
        finally
        {
            EndRefresh();
        }
    }
}
