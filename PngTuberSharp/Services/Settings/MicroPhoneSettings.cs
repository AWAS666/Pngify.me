using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.VisualBasic;
using PngTuberSharp.Helpers;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;

namespace PngTuberSharp.Services.Settings
{
    public class MicroPhoneSettings
    {
        public int ThreshHold { get; set; } = 50;
        public int Device { get; set; } = 0;
        public double BlinkInterval { get; set; } = 2f;
        public double BlinkTime { get; set; } = 0.25f;
        public List<MicroPhoneState> States { get; set; } = new List<MicroPhoneState>()
        {
            new MicroPhoneState()
            {
                Name = "Basic",
                Open = new ImageSetting()
                {
                    FilePath = "Assets/open.png",
                },
                 Closed = new ImageSetting()
                {
                    FilePath = "Assets/closed.png",
                },
            }
        };
    }

    public class MicroPhoneState
    {
        public string Name { get; set; }
        public ImageSetting Open { get; set; } = new();
        public ImageSetting OpenBlink { get; set; } = new();
        public ImageSetting Closed { get; set; } = new();
        public ImageSetting ClosedBlink { get; set; } = new();

        public HotkeyTrigger? Trigger { get; set; } = null;

    }

    public partial class ImageSetting : ObservableObject
    {
        public static SKBitmap PlaceHolder = SKBitmap.Decode(AssetLoader.Open(new Uri("avares://PngTuberSharp/Assets/placeholder.png")));
        private static Bitmap img = new Bitmap(AssetLoader.Open(new Uri("avares://PngTuberSharp/Assets/placeholder.png")));

        private SKBitmap bitmap = PlaceHolder;

        // todo fix why this here cant bind for whatever reason
        private IImage image = img;

        [JsonIgnore]
        public SKBitmap Bitmap
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
                Bitmap = PlaceHolder;
                return;
            }
            if (File.Exists(FilePath))
                Bitmap = SKBitmap.Decode(FilePath);
        }
    }
}
