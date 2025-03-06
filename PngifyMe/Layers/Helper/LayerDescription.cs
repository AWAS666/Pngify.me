using PngifyMe.Lang;
using System;
using System.Resources;

namespace PngifyMe.Layers.Helper;


[AttributeUsage(AttributeTargets.Class)]
public class LayerDescriptionAttribute : Attribute
{
    public string Description => Resources.ResourceManager.GetString(DescriptionKey);
    public string DescriptionKey { get; }

    public LayerDescriptionAttribute(string description)
    {
        DescriptionKey = description;
    }
}
