using System;

namespace PngifyMe.Layers.Helper
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FilePickerAttribute : Attribute
    {
        public string Unit { get; }

        public FilePickerAttribute()
        {
        }
    }
}
