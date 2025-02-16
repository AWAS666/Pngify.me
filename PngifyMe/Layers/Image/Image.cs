using PngifyMe.Layers.Helper;
using PngifyMe.Services.CharacterSetup.Images;
using Serilog;
using SkiaSharp;
using System;
using System.IO;

namespace PngifyMe.Layers.Image
{
    [LayerDescription("Pin an image to your model")]
    public class Image : ImageLayer
    {
        [Unit("pixels (center)")]
        public float PosX { get; set; } = 960;

        [Unit("pixels (center)")]
        public float PosY { get; set; } = 540;

        [Unit("Path")]
        [ImagePicker]
        public string FilePath { get; set; } = string.Empty;

        [Unit("seconds")]
        public float StickyFor { get; set; } = float.MaxValue;

        private BaseImage image;

        public override void OnEnter()
        {
            if (!File.Exists(FilePath))
            {
                Log.Error($"Image: File {FilePath} not found");
                IsExiting = true;
                return;
            }

            AutoRemoveTime = StickyFor;
            image = BaseImage.LoadFromPath(FilePath);
            base.OnEnter();
        }

        public override void OnExit()
        {
            image?.Dispose();
            base.OnExit();
        }

        public override void OnCalculateParameters(float dt, ref LayerValues values)
        {
            // nothing prolly
        }

        public override void RenderImage(SKCanvas canvas, float x, float y)
        {
            if (IsExiting || image == null)
                return;
            var img = GetImage();
            canvas.DrawBitmap(img, PosX - img.Width / 2, PosY - img.Height / 2);
        }

        public SKBitmap GetImage()
        {
            return image.GetImage(TimeSpan.FromSeconds(CurrentTime));
        }
    }
}
