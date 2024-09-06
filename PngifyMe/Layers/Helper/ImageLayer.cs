using PngifyMe.Layers.Helper;
using SkiaSharp;
using System;
using System.Security;

namespace PngifyMe.Layers
{
    public abstract class ImageLayer : PermaLayer
    {
        [Unit("bool")]
        public bool ApplyOtherEffects { get; set; } = false;

        public abstract void RenderImage(SKCanvas canvas, float x, float y);

        public abstract SKBitmap GetImage();

        public ImageLayer()
        {
            ExitTime = 0f;
            EnterTime = 0f;
        }
    }
}
