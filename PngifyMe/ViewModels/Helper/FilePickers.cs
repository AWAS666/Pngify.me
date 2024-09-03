using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngifyMe.ViewModels.Helper
{
    public static class FilePickers
    {
        public static FilePickerFileType ImageAll { get; } = new("All Images")
        {
            Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.gif", "*.bmp", "*.webp" },
            AppleUniformTypeIdentifiers = new[] { "public.image" },
            MimeTypes = new[] { "image/*" }
        };

        public static FilePickerFileType AudioAll { get; } = new("Wav")
        {
            Patterns = new[] { "*.wav", },
            AppleUniformTypeIdentifiers = new[] { "public.audio" },
        };
    }
}
