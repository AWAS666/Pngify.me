using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;

namespace PngTuberSharp.Services.Settings
{
    public class MicroPhoneSettings
    {
        public int ThreshHold { get; set; } = 50;
        public int Device { get; set; } = 1;
        public float BlinkInterval { get; set; } = 2f;
        public float BlinkTime { get; set; } = 0.25f;
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
        static Bitmap PlaceHolder = new Bitmap(AssetLoader.Open(new Uri("avares://PngTuberSharp/Assets/placeholder.png")));

        private Bitmap bitmap = PlaceHolder;

        [JsonIgnore]
        public Bitmap Bitmap
        {
            get
            {
                return bitmap;
            }
            set
            {
                SetProperty(ref bitmap, value);
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
                Bitmap = new Bitmap(FilePath);
        }
    }
}
