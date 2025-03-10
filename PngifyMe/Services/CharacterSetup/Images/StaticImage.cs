using Avalonia.Platform;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using TwitchLib.Api.Helix;

namespace PngifyMe.Services.CharacterSetup.Images;

public class StaticImage : BaseImage
{
    private SKBitmap Bitmap { get; set; }

    public override SKBitmap Preview => Bitmap;

    public override int Width => Bitmap.Width;

    public override int Height => Bitmap.Height;

    public StaticImage(string input, bool loadfromBase64)
    {
        if (loadfromBase64)
        {
            byte[] imageBytes = Convert.FromBase64String(input);
            using var memoryStream = new MemoryStream(imageBytes);
            using var skStream = new SKManagedStream(memoryStream);
            Bitmap = SKBitmap.Decode(skStream);
        }
        else
        {
            Bitmap = SKBitmap.Decode(input);
            if (Bitmap == null)
                throw new Exception("Failed to load image.");
        }
        Bitmap.SetImmutable();
    }

    public StaticImage(SKBitmap bitmap)
    {
        Bitmap = bitmap;
        //https://github.com/mono/SkiaSharp/issues/2188 -> this is a big performance improvment
        Bitmap.SetImmutable();
    }

    public override SKBitmap GetImage(TimeSpan time)
    {
        return Bitmap;
    }

    public override void Resize(int maxWidth, int maxHeight)
    {
        Bitmap = base.Resize(Bitmap, maxWidth, maxHeight);
    }

    public override void Dispose()
    {
        Bitmap.Dispose();
    }

    public override List<string> ConvertToBase64()
    {
        using var imageData = Bitmap.Encode(SKEncodedImageFormat.Png, 100);
        return [Convert.ToBase64String(imageData.ToArray())];
    }

    public static StaticImage LoadFromRessource(string resourceString)
    {
        return new StaticImage(SKBitmap.Decode(AssetLoader.Open(new Uri(resourceString))));
    }
}
