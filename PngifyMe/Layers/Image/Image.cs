using Avalonia.Platform.Storage;
using PngifyMe.Layers.Helper;
using PngifyMe.Services.Settings.Images;
using PngifyMe.ViewModels.Helper;
using Serilog;
using SkiaSharp;
using System;
using System.IO;

namespace PngifyMe.Layers.Image
{
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

            string extension = Path.GetExtension(FilePath).ToLower();
            if (extension == ".gif")
            {
                image = new GifImage(FilePath);
            }
            else if (extension == ".png" || extension == ".jpg" || extension == ".jpeg" || extension == ".bmp")
            {
                image = new StaticImage(FilePath);
            }
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
