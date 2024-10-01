using System;

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
