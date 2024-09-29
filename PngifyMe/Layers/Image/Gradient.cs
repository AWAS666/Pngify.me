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
    public class Gradient : ImageLayer
    {

        public override void OnCalculateParameters(float dt, ref LayerValues values)
        {
            // nothing prolly
        }

        public override void RenderImage(SKCanvas canvas, float offsetX, float offsetY)
        {
            SKRect rect = new SKRect(0, 0, Specsmanager.Width / 2, Specsmanager.Height / 2);
            using SKPaint paint = new();
            // Create linear gradient from upper-left to lower-right
            paint.Shader = SKShader.CreateRadialGradient(
                                new SKPoint(rect.MidX, rect.MidY),
                                100,
                                new SKColor[] { SKColors.Red, SKColors.Blue },
                                null,
                                SKShaderTileMode.Repeat);
            paint.BlendMode = SKBlendMode.Modulate;

            // Draw the gradient on the rectangle
            canvas.DrawRect(rect, paint);

        }
    }
}
