using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Services.CharacterSetup.Images;
using SkiaSharp;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;

namespace PngifyMe.Services.CharacterSetup.Images;

public partial class ImageSetting : ObservableObject
{
    public static SKBitmap PlaceHolder = SKBitmap.Decode(AssetLoader.Open(new Uri("avares://PngifyMeCode/Assets/placeholder.png")));

    [property: JsonIgnore]
    [ObservableProperty]
    private BaseImage bitmap = new StaticImage(PlaceHolder);

    public List<string> Base64
    {
        get => base64; set
        {
            base64 = value;
            LoadBase64();
        }
    }
    private List<string> base64 { get; set; } = new();

    public TimeSpan TimeSpan { get; set; } = TimeSpan.Zero;

    public ImageSetting()
    {
        LoadBase64();
    }

    public void LoadBase64()
    {
        if (base64.Count == 0)
        {
            Bitmap = new StaticImage(PlaceHolder);
            return;
        }
        if (base64.Count == 1)
            Bitmap = new StaticImage(base64[0], true);
        else
            Bitmap = new GifImage(base64, TimeSpan);

        Bitmap.Resize((int)(Specsmanager.Width * Specsmanager.ScaleFactor), (int)(Specsmanager.Height * Specsmanager.ScaleFactor));
    }

    public ImageSetting LoadFromFile(string filePath)
    {
        Bitmap = BaseImage.LoadFromPath(filePath);
        base64 = Bitmap.ConvertToBase64();
        if (Bitmap is GifImage gif)
            TimeSpan = gif.Frames.First().Duration;
        Bitmap.Resize((int)(Specsmanager.Width * Specsmanager.ScaleFactor), (int)(Specsmanager.Height * Specsmanager.ScaleFactor));
        return this;
    }

    public void Delete()
    {
        Base64.Clear();
        LoadBase64();
    }
}
