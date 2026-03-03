using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PngifyMe.Layers.Helper;
using PngifyMe.Services;
using System;
using System.Reflection;

namespace PngifyMe.ViewModels.Helper;

public sealed partial class PositionSizeOverlayViewModel : ObservableObject
{
    private readonly BaseLayerViewModel _layerViewModel;
    private readonly PropertyInfo _propPosition;
    private readonly string _positionPropertyName;

    /// <summary>
    /// We own this instance; read once from layer at init, then update it and push back to the layer.
    /// </summary>
    private readonly CanvasPosition2D _position;
    private bool _isRefreshing;

    public BaseLayerViewModel LayerViewModel => _layerViewModel;

    public PositionSizeOverlayViewModel(BaseLayerViewModel layerViewModel)
    {
        _layerViewModel = layerViewModel ?? throw new ArgumentNullException(nameof(layerViewModel));
        var layerType = layerViewModel.LayerModel.GetType();
        _propPosition = FindPointProperty(layerType)
            ?? throw new InvalidOperationException($"Layer type {layerType.Name} has no property marked with [CanvasPosition].");
        _positionPropertyName = _propPosition.Name;

        var model = _layerViewModel.LayerModel;
        var pos = _propPosition.GetValue(model);
        _position = new CanvasPosition2D
        {
            X = GetFloat(pos, "X"),
            Y = GetFloat(pos, "Y")
        };

        _isRefreshing = true;
        try
        {
            X = _position.X;
            Y = _position.Y;
        }
        finally
        {
            _isRefreshing = false;
        }
    }

    private static PropertyInfo? FindPointProperty(Type type)
    {
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (prop.GetCustomAttribute<CanvasPositionAttribute>() != null)
                return prop;
        }
        return null;
    }

    private static float GetFloat(object? obj, string propertyName)
    {
        if (obj == null) return 0;
        var prop = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        var v = prop?.GetValue(obj);
        return v != null ? Convert.ToSingle(v) : 0;
    }

    [ObservableProperty]
    private float _x;

    [ObservableProperty]
    private float _y;

    private void PushPositionToLayer()
    {
        _position.X = X;
        _position.Y = Y;
        _propPosition.SetValue(_layerViewModel.LayerModel, _position);
        SyncToPropertyList(_positionPropertyName + ".X", X);
        SyncToPropertyList(_positionPropertyName + ".Y", Y);
    }

    private void SyncToPropertyList(string propertyName, float value)
    {
        foreach (var pvm in _layerViewModel.PropertyList)
        {
            if (string.Equals(pvm.Name, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                pvm.Value = value.ToString();
                break;
            }
        }
    }

    partial void OnXChanged(float value)
    {
        if (_isRefreshing) return;
        PushPositionToLayer();
    }
    partial void OnYChanged(float value)
    {
        if (_isRefreshing) return;
        PushPositionToLayer();
    }

    [RelayCommand]
    private void Close()
    {
        CanvasOverlayService.ClearOverlay();
    }
}
