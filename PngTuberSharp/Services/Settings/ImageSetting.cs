using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using PngTuberSharp.Services.Settings.Images;
using SkiaSharp;
using System;
using System.IO;
using System.Text.Json.Serialization;

namespace PngTuberSharp.Services.Settings
{
    public partial class ImageSetting : ObservableObject
    {
        public static SKBitmap PlaceHolder = SKBitmap.Decode(AssetLoader.Open(new Uri("avares://PngTuberSharp/Assets/placeholder.png")));
        private static Bitmap img = new Bitmap(AssetLoader.Open(new Uri("avares://PngTuberSharp/Assets/placeholder.png")));

        private BaseImage bitmap = new StaticImage(PlaceHolder);

        // todo fix why this here cant bind for whatever reason
        private IImage image = img;

        [JsonIgnore]
        public BaseImage Bitmap
        {
            get
            {
                return bitmap;
            }
            set
            {
                bitmap = value;
                if (!string.IsNullOrEmpty(FilePath))
                    Image = new Bitmap(FilePath);
                else
                    Image = img;
            }
        }

        [JsonIgnore]
        public IImage Image
        {
            get
            {
                return image;
            }
            set
            {
                SetProperty(ref image, value);
            }
        }

        public string? FilePath
        {
            get => path; set
            {
                path = value;
                ApplyPath();
            }
        }
        private string? path { get; set; }
        public string? FileName => Path.GetFileName(path);

        public ImageSetting()
        {
            ApplyPath();
        }

        public void ApplyPath()
        {
            if (string.IsNullOrEmpty(FilePath))
            {
                Bitmap = new StaticImage(PlaceHolder);
                return;
            }
            if (File.Exists(FilePath))
            {
                Bitmap = LoadImage(FilePath);
                Bitmap.Resize(1920, 1080);
            }
        }

        public static BaseImage LoadImage(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();

            if (extension == ".gif")
            {
                return new GifImage(filePath);
            }
            else if (extension == ".png" || extension == ".jpg" || extension == ".jpeg" || extension == ".bmp")
            {
                return new StaticImage(filePath);
            }
            else
            {
                throw new NotSupportedException("Unsupported image format.");
            }
        }
    }
}
