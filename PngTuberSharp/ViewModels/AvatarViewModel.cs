using Avalonia.Media;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using PngTuberSharp.Helpers;
using PngTuberSharp.Layers;
using PngTuberSharp.Layers.Microphone;
using PngTuberSharp.Services;
using PngTuberSharp.Services.Settings;
using PngTuberSharp.ViewModels.Helper;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace PngTuberSharp.ViewModels;

public partial class AvatarViewModel : ViewModelBase
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
    private IImage image = ImageSetting.PlaceHolder.ToAvaloniaImage();

    private LayerValues layerValues = new();

    private ObservableCollection<ImageViewModel> imageViewModels = new();
    private SKBitmap cache;

    public AvatarViewModel()
    {
        LayerManager.ValueUpdate += UpdatePosition;
        LayerManager.FPSUpdate += UpdateFPS;
        SettingsManager.Current.LayerSetup.ApplySettings();
    }

    private void UpdateFPS(object? sender, float e)
    {
        Fps = e;
    }

    private void UpdatePosition(object? sender, LayerValues e)
    {
        PosX = e.PosX;
        PosY = e.PosY;
        Rotation = e.Rotation;
        ZoomX = e.ZoomX;
        ZoomY = e.ZoomY;
        Opacity = e.Opacity;
        var watch = new Stopwatch();
        watch.Start();
        Image = e.Image.ToAvaloniaImage();
        //Debug.WriteLine($"Conversion to avalonia took: {watch.Elapsed.TotalMilliseconds}ms");
    }
}
