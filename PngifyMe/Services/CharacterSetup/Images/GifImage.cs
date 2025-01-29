using SkiaSharp;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PngifyMe.Services.CharacterSetup.Images;

public class GifImage : BaseImage
{
    public List<GifFrame> Frames { get; private set; } = new List<GifFrame>();
    public int RepeatCount { get; private set; }

    public override SKBitmap Preview => GetImage(TimeSpan.FromSeconds(0));

    public override int Width => Frames.FirstOrDefault()?.Bitmap.Width ?? 0;

    public override int Height => Frames.FirstOrDefault()?.Bitmap.Height ?? 0;

    public GifImage(string filePath)
    {
        LoadGif(filePath);
    }

    public GifImage(List<string> imageBase64, TimeSpan duration)
    {
        foreach (var frame in imageBase64)
        {
            byte[] imageBytes = Convert.FromBase64String(frame);
            using var memoryStream = new MemoryStream(imageBytes);
            using var skStream = new SKManagedStream(memoryStream);
            var bitmap = SKBitmap.Decode(skStream);
            bitmap.SetImmutable();
            Frames.Add(new GifFrame() { Bitmap = bitmap, Duration = duration });
        }
    }

    private void LoadGif(string filePath)
    {
        byte[] gifBytes = File.ReadAllBytes(filePath);

        using (var codec = SKCodec.Create(new SKMemoryStream(gifBytes)))
        {
            if (codec == null)
                throw new Exception("Failed to load GIF.");

            RepeatCount = codec.RepetitionCount;

            for (int i = 0; i < codec.FrameCount; i++)
            {
                var frameInfo = codec.FrameInfo[i];
                var imageInfo = new SKImageInfo(codec.Info.Width, codec.Info.Height);

                using (var bitmap = new SKBitmap(imageInfo))
                {
                    nint pixels = bitmap.GetPixels();
                    SKCodecOptions options = new SKCodecOptions(i);

                    codec.GetPixels(imageInfo, pixels, options);

                    var bit = bitmap.Copy(); // Make a copy to store in the list
                    bit.SetImmutable();
                    var gifFrame = new GifFrame
                    {
                        Bitmap = bit,
                        Duration = TimeSpan.FromMilliseconds(frameInfo.Duration)
                    };

                    Frames.Add(gifFrame);
                }
            }
        }
    }

    public override SKBitmap GetImage(TimeSpan time)
    {
        if (Frames.Count == 0)
            throw new InvalidOperationException("No frames available in the GIF.");

        double totalDuration = 0;
        foreach (var frame in Frames)
        {
            totalDuration += frame.Duration.TotalMilliseconds;
        }

        double elapsed = time.TotalMilliseconds % totalDuration;
        double accumulatedTime = 0;

        foreach (var frame in Frames)
        {
            accumulatedTime += frame.Duration.TotalMilliseconds;
            if (elapsed < accumulatedTime)
            {
                return frame.Bitmap;
            }
        }

        return Frames[Frames.Count - 1].Bitmap;
    }

    public override void Resize(int maxWidth, int maxHeight)
    {
        foreach (var frame in Frames)
        {
            frame.Bitmap = base.Resize(frame.Bitmap, maxWidth, maxHeight);
            frame.Bitmap.SetImmutable();
        }
    }

    public override void Dispose()
    {
        foreach (var item in Frames)
        {
            item.Bitmap.Dispose();
        }
    }

    public override List<string> ConvertToBase64()
    {
        var ret = new List<string>();
        foreach (var item in Frames)
        {
            using var imageData = item.Bitmap.Encode(SKEncodedImageFormat.Png, 100);
            ret.Add(Convert.ToBase64String(imageData.ToArray()));
        }
        return ret;
    }
}


public class GifFrame
{
    public SKBitmap Bitmap { get; set; }
    public TimeSpan Duration { get; set; }
}
