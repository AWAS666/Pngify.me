using SkiaSharp;
using System;

namespace PngifyMe.Services.Settings.Images
{
    public abstract class BaseImage
    {
        public abstract SKBitmap GetImage(TimeSpan time);

        public abstract void Resize(int maxWidth, int maxHeight);

        public virtual SKBitmap Resize(SKBitmap bitmap, int maxWidth, int maxHeight)
        {
            float widthRatio = (float)maxWidth / bitmap.Width;
            float heightRatio = (float)maxHeight / bitmap.Height;
            float scaleRatio = Math.Min(widthRatio, heightRatio);

            int newWidth = (int)(bitmap.Width * scaleRatio);
            int newHeight = (int)(bitmap.Height * scaleRatio);

            using (var resizedBitmap = new SKBitmap(newWidth, newHeight))
            using (var canvas = new SKCanvas(resizedBitmap))
            {
                canvas.DrawBitmap(bitmap, new SKRect(0, 0, newWidth, newHeight));
                bitmap.Dispose();
                return resizedBitmap.Copy();
            }
        }
    }
}
