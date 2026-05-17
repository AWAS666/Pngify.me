using PngifyMe.Lang;
using PngifyMe.ViewModels.Helper;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PngifyMe.Layers.Helper;

/// <summary>
/// Rectangular region on the canvas. X/Y are center offsets from the canvas center; Width/Height are the box size in pixels.
/// </summary>
public sealed class CanvasRect2D
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
}

/// <summary>
/// Marks a property as editable on the canvas overlay (region). The property type must have X, Y, Width, and Height (e.g. <see cref="CanvasRect2D"/>).
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class CanvasRegionAttribute : Attribute
{
    public static IEnumerable<PropertyViewModel> CreatePropertyViewModels(PropertyInfo prop, object layerModel)
    {
        var regionObj = prop.GetValue(layerModel);
        yield return CreateSubProperty(prop.Name, "X", regionObj, Resources.PixelsCenter, showEditOnCanvas: true);
        yield return CreateSubProperty(prop.Name, "Y", regionObj, Resources.PixelsCenter);
        yield return CreateSubProperty(prop.Name, "Width", regionObj, "pixels");
        yield return CreateSubProperty(prop.Name, "Height", regionObj, "pixels");
    }

    private static PropertyViewModel CreateSubProperty(
        string propertyName, string subName, object? regionObj, string unit, bool showEditOnCanvas = false)
    {
        return new PropertyViewModel
        {
            Name = $"{propertyName}.{subName}",
            Value = GetSubPropertyValue(regionObj, subName)?.ToString() ?? "0",
            Unit = unit,
            Type = typeof(float),
            ShowEditOnCanvasButton = showEditOnCanvas,
            SourcePropertyName = propertyName,
            SourceSubPropertyName = subName,
        };
    }

    private static object? GetSubPropertyValue(object? obj, string subName)
    {
        if (obj == null) return null;
        var sub = obj.GetType().GetProperty(subName, BindingFlags.Public | BindingFlags.Instance);
        return sub?.GetValue(obj);
    }
}
