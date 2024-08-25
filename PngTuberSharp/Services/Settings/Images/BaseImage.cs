using SkiaSharp;
using System;

namespace PngTuberSharp.Services.Settings.Images
{
    public abstract class BaseImage
    {
        public abstract SKBitmap GetImage(TimeSpan time);
    }
}
