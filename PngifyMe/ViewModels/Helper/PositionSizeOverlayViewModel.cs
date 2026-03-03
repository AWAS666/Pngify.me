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
    private bool _isRefreshing;

    public BaseLayerViewModel LayerViewModel => _layerViewModel;

    public PositionSizeOverlayViewModel(BaseLayerViewModel layerViewModel)
    {
        _layerViewModel = layerViewModel ?? throw new ArgumentNullException(nameof(layerViewModel));
        var layerType = layerViewModel.LayerModel.GetType();
        _propPosition = FindPointProperty(layerType)
            ?? throw new InvalidOperationException($"Layer type {layerType.Name} has no property marked with [CanvasPosition].");
        RefreshFromLayer();
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

    [ObservableProperty]
    private float _x;

    [ObservableProperty]
    private float _y;

    public void RefreshFromLayer()
    {
        _isRefreshing = true;
        try
        {
            var model = _layerViewModel.LayerModel;
            var pos = GetPositionObject(model);
            X = GetFloatFromObject(pos, "X");
            Y = GetFloatFromObject(pos, "Y");
        }
        finally
        {
            _isRefreshing = false;
        }
    }

    private object? GetPositionObject(object model)
    {
        return _propPosition.GetValue(model);
    }

    private static float GetFloatFromObject(object? obj, string propertyName)
    {
        if (obj == null) return 0;
        var prop = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        var v = prop?.GetValue(obj);
        return v != null ? Convert.ToSingle(v) : 0;
    }

    private void SetPositionXY(float x, float y)
    {
        var model = _layerViewModel.LayerModel;
        var pos = GetPositionObject(model);
        if (pos == null)
        {
            pos = Activator.CreateInstance(_propPosition.PropertyType);
            if (pos == null) return;
        }
        SetPositionXYOnObject(pos, x, y);
        _propPosition.SetValue(model, pos);
        SyncToPropertyList(_propPosition.Name + ".X", x);
        SyncToPropertyList(_propPosition.Name + ".Y", y);
    }

    private static void SetPositionXYOnObject(object pos, float x, float y)
    {
        var type = pos.GetType();
        var xProp = type.GetProperty("X", BindingFlags.Public | BindingFlags.Instance);
        var yProp = type.GetProperty("Y", BindingFlags.Public | BindingFlags.Instance);
        xProp?.SetValue(pos, Convert.ChangeType(x, xProp.PropertyType));
        yProp?.SetValue(pos, Convert.ChangeType(y, yProp.PropertyType));
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
        SetPositionXY(value, Y);
    }
    partial void OnYChanged(float value)
    {
        if (_isRefreshing) return;
        SetPositionXY(X, value);
    }

    [RelayCommand]
    private void Close()
    {
        CanvasOverlayService.ClearOverlay();
    }
}
