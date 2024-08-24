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
using System.Linq;

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

    [ObservableProperty]
    private ObservableCollection<ImageViewModel> throwables = new();
    private SKBitmap cache;

    public AvatarViewModel()
    {
        LayerManager.ValueUpdate += UpdatePosition;
        LayerManager.FPSUpdate += UpdateFPS;
        LayerManager.ThrowingSystem.UpdateObjects += UpdateObjects;
    }

    private void UpdateObjects(object? sender, float e)
    {
        foreach (var item in LayerManager.ThrowingSystem.Objects)
        {
            var throwable = Throwables.FirstOrDefault(x => x.Item == item);
            if(throwable == null)
            {
                throwable = new ImageViewModel(item)
                {
                    X = item.X,
                    Y = item.Y,
                    Rotation = item.Rotation,
                    Image = item.Image.ToAvaloniaImage()
                };
                Throwables.Add(throwable);
            }

            throwable.X = item.X;
            throwable.Y = item.Y;
            throwable.Rotation = item.Rotation;           
        }
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
        Image = e.Image.ToAvaloniaImage();
      
    }
}
