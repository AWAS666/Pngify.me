using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using PngifyMe.Services;

namespace PngifyMe.Views.Overlay;

public partial class TestOverlayView : UserControl
{
    public TestOverlayView()
    {
        InitializeComponent();
        AddHandler(PointerMovedEvent, OnPointerMoved, Avalonia.Interactivity.RoutingStrategies.Tunnel | Avalonia.Interactivity.RoutingStrategies.Bubble);
        AddHandler(PointerExitedEvent, OnPointerExited, Avalonia.Interactivity.RoutingStrategies.Tunnel | Avalonia.Interactivity.RoutingStrategies.Bubble);
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        var point = e.GetCurrentPoint(this);
        var controlPos = point.Position;
        var bounds = Bounds;
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            mousePosText.Text = $"Mouse: {controlPos.X:F0}, {controlPos.Y:F0}";
            canvasPosText.Text = "Canvas: —";
            return;
        }
        // Canvas origin is top-left; scale control space to canvas space
        double canvasX = (controlPos.X / bounds.Width) * Specsmanager.Width;
        double canvasY = (controlPos.Y / bounds.Height) * Specsmanager.Height;
        mousePosText.Text = $"Mouse: {controlPos.X:F0}, {controlPos.Y:F0}";
        canvasPosText.Text = $"Canvas: {canvasX:F0}, {canvasY:F0}";
    }

    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        mousePosText.Text = "Mouse: —";
        canvasPosText.Text = "Canvas: —";
    }
}
