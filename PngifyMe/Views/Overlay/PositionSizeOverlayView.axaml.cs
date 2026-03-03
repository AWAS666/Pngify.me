using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using PngifyMe.Services;
using System;
using System.ComponentModel;

namespace PngifyMe.Views.Overlay;

public partial class PositionSizeOverlayView : UserControl
{
    private const double TargetGraphicSize = 16;

    public PositionSizeOverlayView()
    {
        InitializeComponent();
        targetCanvas.AddHandler(PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        targetCanvas.AddHandler(PointerMovedEvent, OnPointerMoved, RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        targetCanvas.AddHandler(PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        LayoutUpdated += OnLayoutUpdated;
        closeButton.Click += OnCloseClick;
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (_vmSubscribed != null)
        {
            _vmSubscribed.PropertyChanged -= Vm_PropertyChanged;
            _vmSubscribed = null;
        }
        if (DataContext is ViewModels.Helper.PositionSizeOverlayViewModel vm)
        {
            _vmSubscribed = vm;
            vm.PropertyChanged += Vm_PropertyChanged;
            UpdateTargetPosition();
        }
    }

    private ViewModels.Helper.PositionSizeOverlayViewModel? _vmSubscribed;

    private void Vm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(ViewModels.Helper.PositionSizeOverlayViewModel.X) or nameof(ViewModels.Helper.PositionSizeOverlayViewModel.Y))
            UpdateTargetPosition();
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() => CanvasOverlayService.ClearOverlay());
    }

    private bool _isDragging;

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is not ViewModels.Helper.PositionSizeOverlayViewModel vm)
            return;
        var point = e.GetCurrentPoint(this);
        if (point.Properties.IsLeftButtonPressed)
        {
            _isDragging = true;
            SetPositionFromPointer(point.Position, vm);
            e.Handled = true;
        }
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isDragging || DataContext is not ViewModels.Helper.PositionSizeOverlayViewModel vm)
            return;
        var point = e.GetCurrentPoint(this);
        SetPositionFromPointer(point.Position, vm);
        e.Handled = true;
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased)
            _isDragging = false;
    }

    /// <summary>
    /// Convert control position (top-left origin) to canvas space (top-left origin) and set VM X,Y.
    /// </summary>
    private void SetPositionFromPointer(Point controlPos, ViewModels.Helper.PositionSizeOverlayViewModel vm)
    {
        var bounds = Bounds;
        if (bounds.Width <= 0 || bounds.Height <= 0) return;
        double canvasX = (controlPos.X / bounds.Width) * Specsmanager.Width;
        double canvasY = (controlPos.Y / bounds.Height) * Specsmanager.Height;
        vm.X = (float)canvasX;
        vm.Y = (float)canvasY;
        UpdateTargetPosition();
    }

    private void OnLayoutUpdated(object? sender, EventArgs e)
    {
        UpdateTargetPosition();
    }

    /// <summary>
    /// Convert canvas (X,Y) (top-left origin) to control space and position the target graphic.
    /// </summary>
    private void UpdateTargetPosition()
    {
        if (DataContext is not ViewModels.Helper.PositionSizeOverlayViewModel vm)
            return;
        var bounds = Bounds;
        if (bounds.Width <= 0 || bounds.Height <= 0) return;
        double controlX = (vm.X / Specsmanager.Width) * bounds.Width;
        double controlY = (vm.Y / Specsmanager.Height) * bounds.Height;
        Canvas.SetLeft(targetGraphic, controlX - TargetGraphicSize / 2);
        Canvas.SetTop(targetGraphic, controlY - TargetGraphicSize / 2);
    }
}
