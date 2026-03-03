using System;

namespace PngifyMe.Layers.Helper;

/// <summary>
/// Marks a property as editable on the canvas overlay (position). Use [CanvasPosition] on a single property of type with X and Y (e.g. CanvasPosition2D), or [CanvasPosition(Role)] for separate X/Y properties. Bounding box (Width/Height) will use a separate attribute later.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class CanvasPositionAttribute : Attribute
{
    public CanvasPositionRole Role { get; }
    public bool IsPointProperty { get; }

    /// <summary>Marks this property as the position point (type must have X and Y).</summary>
    public CanvasPositionAttribute()
    {
        IsPointProperty = true;
        Role = CanvasPositionRole.X;
    }

    /// <summary>Marks this property as the given role (X or Y).</summary>
    public CanvasPositionAttribute(CanvasPositionRole role)
    {
        IsPointProperty = false;
        Role = role;
    }
}

/// <summary>Role for canvas position (X/Y only). Bounding box (Width/Height) will use a separate attribute later.</summary>
public enum CanvasPositionRole
{
    X,
    Y
}
