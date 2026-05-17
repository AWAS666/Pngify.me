using PngifyMe.Lang;
using PngifyMe.ViewModels.Helper;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PngifyMe.Layers.Helper;

/// <summary>
/// Marks a single property as editable on the canvas overlay (position). The property type must have X and Y (e.g. <see cref="CanvasPosition2D"/>).
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class CanvasPositionAttribute : Attribute
{
    public static IEnumerable<PropertyViewModel> CreatePropertyViewModels(PropertyInfo prop, object layerModel)
    {
        var posObj = prop.GetValue(layerModel);
        yield return CreateSubProperty(prop.Name, "X", posObj, Resources.PixelsCenter, showEditOnCanvas: true);
        yield return CreateSubProperty(prop.Name, "Y", posObj, Resources.PixelsCenter);
    }

    private static PropertyViewModel CreateSubProperty(
        string propertyName, string subName, object? posObj, string unit, bool showEditOnCanvas = false)
    {
        return new PropertyViewModel
        {
            Name = $"{propertyName}.{subName}",
            Value = GetSubPropertyValue(posObj, subName)?.ToString() ?? "0",
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
