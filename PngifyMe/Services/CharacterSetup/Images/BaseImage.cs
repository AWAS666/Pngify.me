using CppSharp.Types.Std;
using PngifyMe.Helpers;
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
        return SkiaHelper.Resize(bitmap, maxWidth, maxHeight);       
    }

    public static BaseImage LoadFromPath(string path)
    {
        string extension = Path.GetExtension(path).ToLower();
        if (extension == ".gif")
        {
            return new GifImage(path);
        }
        else if (extension == ".png" || extension == ".jpg" || extension == ".jpeg" || extension == ".bmp")
        {
            return new StaticImage(path, false);
        }
        else
        {
            throw new NotSupportedException("Unsupported image format.");
        }
    }


}
