using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngifyMe.Helpers;

public static class SkiaHelper
{
    /// <summary>
    /// resizes bitmap and keeps aspect ratio
    /// </summary>
    /// <param name="bitmap"></param>
    /// <param name="maxWidth"></param>
    /// <param name="maxHeight"></param>
    /// <returns></returns>
    public static SKBitmap Resize(SKBitmap bitmap, int maxWidth, int maxHeight)
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
