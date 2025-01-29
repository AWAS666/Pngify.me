using CppSharp.Types.Std;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace PngifyMe.Services.CharacterSetup.Images;

public abstract class BaseImage : IDisposable
{
    public abstract void Dispose();
    public abstract SKBitmap GetImage(TimeSpan time);
    public abstract SKBitmap Preview { get; }
    public abstract int Width { get; }
    public abstract int Height { get; }

    public abstract void Resize(int maxWidth, int maxHeight);
    public abstract List<string> ConvertToBase64();

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

    public static BaseImage LoadFromPath(string path)
    {
        string extension = Path.GetExtension(path);
        if (extension == ".gif")
        {
            return new GifImage(path);
        }
        else if (extension == ".png" || extension == ".jpg" || extension == ".jpeg" || extension == ".bmp")
        {
            return new StaticImage(path);
        }
        else
        {
            throw new NotSupportedException("Unsupported image format.");
        }
    }


}
