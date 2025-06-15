using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Helpers;
using PngifyMe.Layers;
using SkiaSharp;
using System;

namespace PngifyMe.ViewModels;

public partial class AvatarViewModel : ObservableObject
{
    [ObservableProperty]
    private double posX;

    [ObservableProperty]
    private double posY;

    [ObservableProperty]
    private double rotation;

    [ObservableProperty]
    private double zoomX;

    [ObservableProperty]
    private double zoomY;

    [ObservableProperty]
    private double opacity;

    [ObservableProperty]
    private float fps;

    [ObservableProperty]
    private SaveDispose<SKBitmap> skImage;

    public AvatarViewModel()
    {
        LayerManager.ImageUpdate += UpdateImage;
        LayerManager.FPSUpdate += UpdateFPS;
    }

    private void UpdateImage(object? sender, SaveDispose<SKBitmap> e)
    {
        SkImage = LayerManager.CurrentFrame;
    }

    private void UpdateFPS(object? sender, float e)
    {
        Fps = MathF.Round(e, 1);
    }
}
