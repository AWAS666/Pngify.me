using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using PngifyMe.Services;
using PngifyMe.ViewModels.Helper;
using System;
using System.ComponentModel;

namespace PngifyMe.Views.Overlay;

public partial class CropRegionOverlayView : UserControl
{
    private enum DragMode { None, Move, Resize }

    private const double HandleVisualSize = 10;
    private const double HandleHitRadius = 14;

    private DragMode _dragMode;
    private ResizeHandle _resizeHandle;
    private Point _dragStartControl;
    private double _startLeft;
    private double _startTop;
    private double _startW;
    private double _startH;
    private double _previewLeft;
    private double _previewTop;
    private double _previewW;
    private double _previewH;

    public CropRegionOverlayView()
    {
        InitializeComponent();
        targetCanvas.AddHandler(PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        targetCanvas.AddHandler(PointerMovedEvent, OnPointerMoved, RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        targetCanvas.AddHandler(PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        LayoutUpdated += OnLayoutUpdated;
        closeButton.Click += OnCloseClick;
        DataContextChanged += OnDataContextChanged;
    }

    private CropRegionOverlayViewModel? _vmSubscribed;

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (_vmSubscribed != null)
        {
            _vmSubscribed.PropertyChanged -= Vm_PropertyChanged;
            _vmSubscribed = null;
        }
        if (DataContext is CropRegionOverlayViewModel vm)
        {
            _vmSubscribed = vm;
            vm.PropertyChanged += Vm_PropertyChanged;
            UpdateTargetRect();
        }
    }

    private void Vm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(CropRegionOverlayViewModel.X)
            or nameof(CropRegionOverlayViewModel.Y)
            or nameof(CropRegionOverlayViewModel.Width)
            or nameof(CropRegionOverlayViewModel.Height))
        {
            UpdateTargetRect();
        }
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() => CanvasOverlayService.ClearOverlay());
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is not CropRegionOverlayViewModel vm)
            return;

        var point = e.GetCurrentPoint(this);
        if (!point.Properties.IsLeftButtonPressed)
            return;

        var bounds = Bounds;
        if (bounds.Width <= 0 || bounds.Height <= 0)
            return;

        var (left, top, w, h) = vm.RectToControl(bounds.Width, bounds.Height, Specsmanager.Width, Specsmanager.Height);
        var pos = point.Position;

        _resizeHandle = HitTestHandle(left, top, w, h, pos);
        if (_resizeHandle != ResizeHandle.None)
        {
            _dragMode = DragMode.Resize;
            _previewLeft = left;
            _previewTop = top;
            _previewW = w;
            _previewH = h;
        }
        else if (pos.X >= left && pos.X <= left + w && pos.Y >= top && pos.Y <= top + h)
        {
            _dragMode = DragMode.Move;
        }
        else
        {
            return;
        }

        _dragStartControl = pos;
        _startLeft = left;
        _startTop = top;
        _startW = w;
        _startH = h;
        e.Handled = true;
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_dragMode == DragMode.None || DataContext is not CropRegionOverlayViewModel vm)
            return;

        var bounds = Bounds;
        if (bounds.Width <= 0 || bounds.Height <= 0)
            return;

        var point = e.GetCurrentPoint(this);
        var delta = point.Position - _dragStartControl;

        if (_dragMode == DragMode.Move)
        {
            var (scale, _, _) = CanvasPointOverlayViewModelBase.GetBitmapRectInControl(
                bounds.Width, bounds.Height, Specsmanager.Width, Specsmanager.Height);
            if (scale <= 0) return;
            vm.X += (float)(delta.X / scale);
            vm.Y += (float)(delta.Y / scale);
            _dragStartControl = point.Position;
        }
        else
        {
            ApplyResizeToPreview(delta);
        }

        UpdateTargetRect();
        e.Handled = true;
    }

    private void ApplyResizeToPreview(Vector delta)
    {
        double left = _startLeft;
        double top = _startTop;
        double w = _startW;
        double h = _startH;

        switch (_resizeHandle)
        {
            case ResizeHandle.TopLeft:
                left += delta.X;
                top += delta.Y;
                w -= delta.X;
                h -= delta.Y;
                break;
            case ResizeHandle.Top:
                top += delta.Y;
                h -= delta.Y;
                break;
            case ResizeHandle.TopRight:
                top += delta.Y;
                w += delta.X;
                h -= delta.Y;
                break;
            case ResizeHandle.Right:
                w += delta.X;
                break;
            case ResizeHandle.BottomRight:
                w += delta.X;
                h += delta.Y;
                break;
            case ResizeHandle.Bottom:
                h += delta.Y;
                break;
            case ResizeHandle.BottomLeft:
                left += delta.X;
                w -= delta.X;
                h += delta.Y;
                break;
            case ResizeHandle.Left:
                left += delta.X;
                w -= delta.X;
                break;
        }

        if (w < 1)
        {
            left += w - 1;
            w = 1;
        }
        if (h < 1)
        {
            top += h - 1;
            h = 1;
        }

        _previewLeft = left;
        _previewTop = top;
        _previewW = w;
        _previewH = h;
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.PointerUpdateKind != PointerUpdateKind.LeftButtonReleased)
            return;

        if (_dragMode == DragMode.Resize && DataContext is CropRegionOverlayViewModel vm)
        {
            var bounds = Bounds;
            if (bounds.Width > 0 && bounds.Height > 0)
            {
                vm.SetPushSizeChangesToModel(false);
                try
                {
                    vm.ControlRectToLogical(_previewLeft, _previewTop, _previewW, _previewH,
                        bounds.Width, bounds.Height, Specsmanager.Width, Specsmanager.Height);
                }
                finally
                {
                    vm.SetPushSizeChangesToModel(true);
                }
                vm.CommitToModel();
            }
        }

        _dragMode = DragMode.None;
        _resizeHandle = ResizeHandle.None;
    }

    private void OnLayoutUpdated(object? sender, EventArgs e) => UpdateTargetRect();

    private void UpdateTargetRect()
    {
        if (DataContext is not CropRegionOverlayViewModel vm)
            return;

        var bounds = Bounds;
        if (bounds.Width <= 0 || bounds.Height <= 0)
            return;

        double left, top, w, h;
        if (_dragMode == DragMode.Resize)
        {
            left = _previewLeft;
            top = _previewTop;
            w = _previewW;
            h = _previewH;
        }
        else
        {
            (left, top, w, h) = vm.RectToControl(bounds.Width, bounds.Height, Specsmanager.Width, Specsmanager.Height);
        }

        Canvas.SetLeft(targetGraphic, left);
        Canvas.SetTop(targetGraphic, top);
        targetGraphic.Width = Math.Max(1, w);
        targetGraphic.Height = Math.Max(1, h);

        PositionHandle(handleTopLeft, left, top);
        PositionHandle(handleTop, left + w / 2, top);
        PositionHandle(handleTopRight, left + w, top);
        PositionHandle(handleRight, left + w, top + h / 2);
        PositionHandle(handleBottomRight, left + w, top + h);
        PositionHandle(handleBottom, left + w / 2, top + h);
        PositionHandle(handleBottomLeft, left, top + h);
        PositionHandle(handleLeft, left, top + h / 2);
    }

    private static void PositionHandle(Ellipse handle, double centerX, double centerY)
    {
        Canvas.SetLeft(handle, centerX - HandleVisualSize / 2);
        Canvas.SetTop(handle, centerY - HandleVisualSize / 2);
    }

    private static ResizeHandle HitTestHandle(double left, double top, double w, double h, Point pos)
    {
        var handles = new (ResizeHandle kind, double x, double y)[]
        {
            (ResizeHandle.TopLeft, left, top),
            (ResizeHandle.TopRight, left + w, top),
            (ResizeHandle.BottomRight, left + w, top + h),
            (ResizeHandle.BottomLeft, left, top + h),
            (ResizeHandle.Top, left + w / 2, top),
            (ResizeHandle.Bottom, left + w / 2, top + h),
            (ResizeHandle.Left, left, top + h / 2),
            (ResizeHandle.Right, left + w, top + h / 2),
        };

        foreach (var (kind, hx, hy) in handles)
        {
            if (Math.Abs(pos.X - hx) <= HandleHitRadius && Math.Abs(pos.Y - hy) <= HandleHitRadius)
                return kind;
        }

        return ResizeHandle.None;
    }

    private enum ResizeHandle
    {
        None,
        TopLeft,
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        BottomLeft,
        Left
    }
}
