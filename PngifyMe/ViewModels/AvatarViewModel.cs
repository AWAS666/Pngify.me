using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Layers;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;

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
    private SKImage skImage;
    private List<SKImage> oldFrame = new();

    public AvatarViewModel()
    {
        LayerManager.ImageUpdate += UpdateImage;
        LayerManager.FPSUpdate += UpdateFPS;
    }

    private void UpdateImage(object? sender, SKImage e)
    {
        SkImage = e;

        //oldFrame.Add(e);
        //if (oldFrame.Count > 4)
        //    foreach (var frame in oldFrame.Take(1))
        //    {
        //        frame.Dispose();
        //        oldFrame.Remove(frame);
        //    }
    }

    private void UpdateFPS(object? sender, float e)
    {
        Fps = e;
    }    
}
