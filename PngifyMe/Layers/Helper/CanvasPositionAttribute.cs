using System;

namespace PngifyMe.Layers.Helper;

/// <summary>
/// Marks a single property as editable on the canvas overlay (position). The property type must have X and Y (e.g. <see cref="CanvasPosition2D"/>). Bounding box (Width/Height) will use a separate attribute later.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class CanvasPositionAttribute : Attribute
{
}
