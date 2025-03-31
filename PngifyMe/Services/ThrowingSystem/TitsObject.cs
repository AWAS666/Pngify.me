using Avalonia.Platform;
using SkiaSharp;
using System;
using System.IO;

namespace PngifyMe.Services.ThrowingSystem;

public class TitsObject
{
    public string Name { get; set; }
    public SKBitmap Bitmap { get; set; }

    public static TitsObject Decode(string url)
    {
        return new()
        {
            Name = Path.GetFileName(url),
            Bitmap = SKBitmap.Decode(AssetLoader.Open(new Uri(url))).Resize(new SKSizeI(Specsmanager.TitsSize, Specsmanager.TitsSize), SKSamplingOptions.Default)
        };
    }

    public static TitsObject Decode(string name, SKMemoryStream stream)
    {
        return new()
        {
            Name = name,
            Bitmap = SKBitmap.Decode(stream).Resize(new SKSizeI(Specsmanager.TitsSize, Specsmanager.TitsSize), SKSamplingOptions.Default)
        };
    }
}
