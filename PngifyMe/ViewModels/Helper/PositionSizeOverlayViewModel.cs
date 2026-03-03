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
    private readonly PropertyInfo? _propPosition;
    private readonly PropertyInfo? _propX;
    private readonly PropertyInfo? _propY;
    private bool _isRefreshing;

    public BaseLayerViewModel LayerViewModel => _layerViewModel;

    public PositionSizeOverlayViewModel(BaseLayerViewModel layerViewModel)
    {
        _layerViewModel = layerViewModel ?? throw new ArgumentNullException(nameof(layerViewModel));
        var layerType = layerViewModel.LayerModel.GetType();
        _propPosition = FindPointProperty(layerType);
        if (_propPosition == null)
        {
            _propX = FindPropertyByRole(layerType, CanvasPositionRole.X);
            _propY = FindPropertyByRole(layerType, CanvasPositionRole.Y);
        }
        else
        {
            _propX = null;
            _propY = null;
        }
        RefreshFromLayer();
    }

    private static PropertyInfo? FindPointProperty(Type type)
    {
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var attr = prop.GetCustomAttribute<CanvasPositionAttribute>();
            if (attr?.IsPointProperty == true)
                return prop;
        }
        return null;
    }

    private static PropertyInfo? FindPropertyByRole(Type type, CanvasPositionRole role)
    {
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var attr = prop.GetCustomAttribute<CanvasPositionAttribute>();
            if (attr != null && !attr.IsPointProperty && attr.Role == role)
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
            if (_propPosition != null)
            {
                var pos = GetPositionObject(model);
                X = GetFloatFromObject(pos, "X");
                Y = GetFloatFromObject(pos, "Y");
            }
            else
            {
                X = GetFloat(_propX, model);
                Y = GetFloat(_propY, model);
            }
        }
        finally
        {
            _isRefreshing = false;
        }
    }

    private object? GetPositionObject(object model)
    {
        return _propPosition?.GetValue(model);
    }

    private static float GetFloatFromObject(object? obj, string propertyName)
    {
        if (obj == null) return 0;
        var prop = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        var v = prop?.GetValue(obj);
        return v != null ? Convert.ToSingle(v) : 0;
    }

    private static float GetFloat(PropertyInfo? prop, object model)
    {
        if (prop == null) return 0;
        var v = prop.GetValue(model);
        return v != null ? Convert.ToSingle(v) : 0;
    }

    private void SetLayerValue(PropertyInfo? prop, float value, string propertyName)
    {
        if (prop == null) return;
        var model = _layerViewModel.LayerModel;
        var converted = Convert.ChangeType(value, prop.PropertyType);
        prop.SetValue(model, converted);
        SyncToPropertyList(propertyName, value);
    }

    private void SetPositionXY(float x, float y)
    {
        if (_propPosition == null) return;
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
        if (_propPosition != null)
            SetPositionXY(value, Y);
        else
            SetLayerValue(_propX, value, _propX?.Name ?? "PosX");
    }
    partial void OnYChanged(float value)
    {
        if (_isRefreshing) return;
        if (_propPosition != null)
            SetPositionXY(X, value);
        else
            SetLayerValue(_propY, value, _propY?.Name ?? "PosY");
    }

    [RelayCommand]
    private void Close()
    {
        CanvasOverlayService.ClearOverlay();
    }
}
