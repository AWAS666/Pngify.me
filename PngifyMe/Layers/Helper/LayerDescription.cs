using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngifyMe.Layers.Helper
{

    [AttributeUsage(AttributeTargets.Class)]
    public class LayerDescriptionAttribute : Attribute
    {
        public string Description { get; }
        public LayerDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
