using PngifyMe.Layers.Helper;
using SkiaSharp;
using System;
using System.Security;

namespace PngifyMe.Layers
{
    public abstract class ImageLayer : PermaLayer
    {
        public abstract void RenderImage(SKCanvas canvas);

        public abstract SKBitmap GetImage();

        public ImageLayer()
        {
            ExitTime = 0f;
            EnterTime = 0f;
        }
    }
}
