using PngifyMe.Layers.Helper;
using PngifyMe.Services;
using SkiaSharp;
using System;

namespace PngifyMe.Layers.Image
{
    [LayerDescription("Darken your image")]

    public class Darken : ImageLayer
    {
        private SKBitmap bitmap;
        private int framecounter;

        public override void OnCalculateParameters(float dt, ref LayerValues values)
        {
            // nothing prolly
        }

        public override void RenderImage(SKCanvas canvas, float offsetX, float offsetY)
        {
            // Cache the noise bitmap (generate once, reuse every frame)
            if (bitmap == null)
                bitmap = new SKBitmap(Specsmanager.Width, Specsmanager.Height);
            // 3x 255 is nothing
            // 3x 0 is all black
            byte count = (byte)Math.Max(100, 255 - framecounter);
            FillBitmapWithColor(bitmap, new SKColor(count, count, count, 255));

            var grainPaint = new SKPaint
            {
                BlendMode = SKBlendMode.Modulate
            };

            canvas.DrawBitmap(bitmap, new SKRect(0, 0, bitmap.Width, bitmap.Height), grainPaint);

            framecounter += 2;
        }

        void FillBitmapWithColor(SKBitmap bitmap, SKColor color)
        {
            using var canvas = new SKCanvas(bitmap);
            // Set the paint to the desired color
            using var paint = new SKPaint { Color = color };
            // Draw a rectangle covering the entire bitmap
            canvas.DrawRect(new SKRect(0, 0, bitmap.Width, bitmap.Height), paint);
        }
    }
}
