using PngifyMe.Layers.Helper;
using System;
using System.Reflection;

namespace PngifyMe.ViewModels.Helper;

public sealed partial class CropRegionOverlayViewModel : CanvasRegionOverlayViewModelBase
{
    private readonly BaseLayerViewModel _layerViewModel;
    private readonly PropertyInfo _propRegion;
    private readonly string _regionPropertyName;
    private readonly CanvasRect2D _region;

    public BaseLayerViewModel LayerViewModel => _layerViewModel;

    public CropRegionOverlayViewModel(BaseLayerViewModel layerViewModel)
    {
        _layerViewModel = layerViewModel ?? throw new ArgumentNullException(nameof(layerViewModel));
        var layerType = layerViewModel.LayerModel.GetType();
        _propRegion = FindRegionProperty(layerType)
            ?? throw new InvalidOperationException($"Layer type {layerType.Name} has no property marked with [CanvasRegion].");
        _regionPropertyName = _propRegion.Name;

        var model = _layerViewModel.LayerModel;
        var regionObj = _propRegion.GetValue(model);
        _region = new CanvasRect2D
        {
            X = GetFloat(regionObj, "X"),
            Y = GetFloat(regionObj, "Y"),
            Width = GetFloat(regionObj, "Width"),
            Height = GetFloat(regionObj, "Height"),
        };

        BeginRefresh();
        try
        {
            X = _region.X;
            Y = _region.Y;
            Width = _region.Width;
            Height = _region.Height;
        }
        finally
        {
            EndRefresh();
        }
    }

    private static PropertyInfo? FindRegionProperty(Type type)
    {
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (prop.GetCustomAttribute<CanvasRegionAttribute>() != null)
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

    protected override void PushToModel()
    {
        _region.X = X;
        _region.Y = Y;
        _region.Width = Width;
        _region.Height = Height;
        _propRegion.SetValue(_layerViewModel.LayerModel, _region);
        SyncToPropertyList(_regionPropertyName + ".X", X);
        SyncToPropertyList(_regionPropertyName + ".Y", Y);
        SyncToPropertyList(_regionPropertyName + ".Width", Width);
        SyncToPropertyList(_regionPropertyName + ".Height", Height);
        SyncToPropertyList("CenterOffsetX", X);
        SyncToPropertyList("CenterOffsetY", Y);
        SyncToPropertyList("Width", Width);
        SyncToPropertyList("Height", Height);
    }

    private void SyncToPropertyList(string propertyName, float value) =>
        SyncToPropertyList(propertyName, value.ToString());

    private void SyncToPropertyList(string propertyName, string value)
    {
        foreach (var pvm in _layerViewModel.PropertyList)
        {
            if (string.Equals(pvm.Name, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                pvm.Value = value;
                break;
            }
        }
    }
}
