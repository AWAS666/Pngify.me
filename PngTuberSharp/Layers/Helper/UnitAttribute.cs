using System;

namespace PngTuberSharp.Layers.Helper
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UnitAttribute : Attribute
    {
        public string Unit { get; }

        public UnitAttribute(string unit)
        {
            Unit = unit;
        }
    }
}
