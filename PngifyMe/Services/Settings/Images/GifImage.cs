using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace PngifyMe.Services.Settings.Images
{
    public class GifImage : BaseImage
    {
        public List<GifFrame> Frames { get; private set; } = new List<GifFrame>();
        public int RepeatCount { get; private set; }

        public GifImage(string filePath)
        {
            LoadGif(filePath);
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
                        IntPtr pixels = bitmap.GetPixels();
                        SKCodecOptions options = new SKCodecOptions(i);

                        codec.GetPixels(imageInfo, pixels, options);

                        var gifFrame = new GifFrame
                        {
                            Bitmap = bitmap.Copy(), // Make a copy to store in the list
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
            }
        }
    }
}
