﻿using Avalonia.Platform.Storage;

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

        public static FilePickerFileType Zip { get; } = new("zip")
        {
            Patterns = new[] { "*.zip", },
            AppleUniformTypeIdentifiers = new[] { "public.archive" },
        };
    }
}
