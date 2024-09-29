using Avalonia.Platform.Storage;
using PngifyMe.Layers.Helper;
using PngifyMe.Services;
using PngifyMe.Services.Settings.Images;
using PngifyMe.ViewModels.Helper;
using SkiaSharp;
using System;
using System.IO;

namespace PngifyMe.Layers.Image
{
    public class FilmGrain : ImageLayer
    {
        private SKBitmap cachedNoiseBitmap;
        private int frameCounter;

        public float Intensity { get; set; } = 0.2f;


        public override void OnCalculateParameters(float dt, ref LayerValues values)
        {
            // nothing prolly
        }

        public override void RenderImage(SKCanvas canvas, float offsetX, float offsetY)
        {
            var random = new Random();

            // Cache the noise bitmap (generate once, reuse every frame)
            if (cachedNoiseBitmap == null)
            {
                int noiseWidth = 256; // Smaller noise texture size
                int noiseHeight = 256;

                cachedNoiseBitmap = new SKBitmap(noiseWidth, noiseHeight);

                // Generate the noise once and store it in the cache
                for (int y = 0; y < cachedNoiseBitmap.Height; y++)
                {
                    for (int x = 0; x < cachedNoiseBitmap.Width; x++)
                    {
                        byte noise = (byte)(random.Next(256) * Intensity); // Adjust grain intensity
                        cachedNoiseBitmap.SetPixel(x, y, new SKColor(noise, noise, noise, 255));
                    }
                }
            }

            var grainPaint = new SKPaint
            {
                BlendMode = SKBlendMode.Modulate // Only apply grain where the canvas has content (no alpha)
            };

            // Calculate noise tile position for slight shifting effect between frames (optional)
            int shiftX = frameCounter % cachedNoiseBitmap.Width;
            int shiftY = frameCounter % cachedNoiseBitmap.Height;

            // Draw the cached noise bitmap repeatedly to cover the entire canvas size (for wrapping)
            var destRect = new SKRect(0, 0, Specsmanager.Width, Specsmanager.Height);
            for (int y = -shiftY; y < Specsmanager.Height; y += cachedNoiseBitmap.Height)
            {
                for (int x = -shiftX; x < Specsmanager.Width; x += cachedNoiseBitmap.Width)
                {
                    canvas.DrawBitmap(cachedNoiseBitmap, new SKRect(x, y, x + cachedNoiseBitmap.Width, y + cachedNoiseBitmap.Height), grainPaint);
                }
            }

            // Increment frame counter to slightly shift the noise between frames
            frameCounter++;
        }
    }
}
