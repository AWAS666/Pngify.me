using Avalonia.Platform.Storage;
using PngifyMe.ViewModels.Helper;
using System;

namespace PngifyMe.Layers.Helper
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FilePickerAttribute : Attribute
    {
        public FilePickerFileType Type { get; }
        public FilePickerAttribute(FilePickerFileType type)
        {
            Type = type;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ImagePickerAttribute : FilePickerAttribute
    {
        public ImagePickerAttribute() : base(FilePickers.ImageAll)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class WavPickerAttribute : FilePickerAttribute
    {
        public WavPickerAttribute() : base(FilePickers.AudioAll)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class FolderPickerAttribute : Attribute
    {
        public FolderPickerAttribute()
        {
        }
    }
}
