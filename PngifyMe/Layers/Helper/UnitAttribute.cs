using System;

namespace PngifyMe.Layers.Helper;

[AttributeUsage(AttributeTargets.Property)]
public class UnitAttribute : Attribute
{
    public string Unit { get; }

    public UnitAttribute(string unit)
    {
        Unit = unit;
    }
}
