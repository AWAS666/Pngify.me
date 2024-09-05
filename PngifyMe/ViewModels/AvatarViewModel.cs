using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Layers;
using SkiaSharp;

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

    public AvatarViewModel()
    {
        LayerManager.ImageUpdate += UpdateImage;
        LayerManager.FPSUpdate += UpdateFPS;
    }

    private void UpdateImage(object? sender, SKImage e)
    {
        SkImage = e;
    }

    private void UpdateFPS(object? sender, float e)
    {
        Fps = e;
    }    
}
