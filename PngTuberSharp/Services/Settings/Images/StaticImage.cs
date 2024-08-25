using SkiaSharp;
using System;

namespace PngTuberSharp.Services.Settings.Images
{
    public class StaticImage : BaseImage
    {
        private SKBitmap Bitmap { get; set; }

        public StaticImage(string filePath)
        {
            Bitmap = SKBitmap.Decode(filePath);
            if (Bitmap == null)
                throw new Exception("Failed to load image.");
        }

        public StaticImage(SKBitmap placeHolder)
        {
            Bitmap = placeHolder;
        }

        public override SKBitmap GetImage(TimeSpan time)
        {
            return Bitmap;
        }

        public override void Resize(int maxWidth, int maxHeight)
        {
            Bitmap = base.Resize(Bitmap, maxWidth, maxHeight);
        }
    }
}
