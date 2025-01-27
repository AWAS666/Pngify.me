﻿using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Services.Settings.Images;
using SkiaSharp;
using System;
using System.IO;
using System.Text.Json.Serialization;

namespace PngifyMe.Services.Settings
{
    public partial class ImageSetting : ObservableObject
    {
        public static SKBitmap PlaceHolder = SKBitmap.Decode(AssetLoader.Open(new Uri("avares://PngifyMeCode/Assets/placeholder.png")));

        [property: JsonIgnore]
        [ObservableProperty]
        private BaseImage bitmap = new StaticImage(PlaceHolder);

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
                //todo: make this scaling here a setting
                Bitmap.Resize((int)(Specsmanager.Width * 0.9f), (int)(Specsmanager.Height * 0.9f));
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
