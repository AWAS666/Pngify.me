using System;

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
}
