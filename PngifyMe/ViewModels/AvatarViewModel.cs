using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Helpers;
using PngifyMe.Layers;
using PngifyMe.Services.Settings;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using static PngifyMe.Helpers.SkiaExtensions;

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
    private AvaloniaImage image = (AvaloniaImage)ImageSetting.PlaceHolder.ToAvaloniaImage();


    private List<AvaloniaImage> oldImages = new();

    public EventHandler RequestRedraw;

    public AvatarViewModel()
    {
        LayerManager.ImageUpdate += UpdateImage;
        LayerManager.FPSUpdate += UpdateFPS;
    }

    private void UpdateImage(object? sender, SKImage e)
    {
        Image.UpdateImage(e);
        RequestRedraw?.Invoke(this, new EventArgs());
    }

    private void UpdateFPS(object? sender, float e)
    {
        Fps = e;
    }    
}
