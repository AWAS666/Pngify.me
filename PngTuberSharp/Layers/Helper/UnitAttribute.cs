using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
