﻿using Avalonia.Media;
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using static PngTuberSharp.Helpers.SkiaExtensions;

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
    private IImage image = (AvaloniaImage)ImageSetting.PlaceHolder.ToAvaloniaImage();


    private List<AvaloniaImage> oldImages = new();

    public AvatarViewModel()
    {
        LayerManager.ValueUpdate += UpdatePosition;
        LayerManager.FPSUpdate += UpdateFPS;
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
      

        // this is hacky af, but else the memory gets filled with the newly generated drawings
        var img = (AvaloniaImage)e.Image.ToAvaloniaImage();
        Image = img;
        oldImages.Add(img);

        if (oldImages.Count > 15)
        {
            foreach (var image in oldImages.Take(5).ToList())
            {
                oldImages.Remove(image);
                image.Dispose();
            }
        }
    }
}
