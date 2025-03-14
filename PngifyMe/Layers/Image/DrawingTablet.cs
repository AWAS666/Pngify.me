﻿using Avalonia.Platform;
using PngifyMe.Layers.Helper;
using PngifyMe.Services;
using PngifyMe.Services.CharacterSetup.Images;
using PngifyMe.Services.Hotkey;
using Serilog;
using SharpHook;
using SkiaSharp;
using System;

namespace PngifyMe.Layers.Image;

[LayerDescription("DrawingTablet")]
public class DrawingTablet : ImageLayer
{
    private static SKBitmap defaulttablet;
    private static SKBitmap defaultstylus;
    private BaseImage tablet;
    private BaseImage stylus;
    private short x;
    private short y;
    private int count;

    [Unit("pixel")]
    public int ScreenMinX { get; set; } = 0;

    [Unit("pixel")]
    public int ScreenMaxX { get; set; } = Specsmanager.Width;

    [Unit("pixel")]
    public int ScreenMinY { get; set; } = 0;

    [Unit("pixel")]
    public int ScreenMaxY { get; set; } = Specsmanager.Height;

    [Unit("Path")]
    [ImagePicker]
    public string TabletPath { get; set; } = string.Empty;

    [Unit("Path")]
    [ImagePicker]
    public string StylusPath { get; set; } = string.Empty;

    [Unit("%")]
    public int Scale { get; set; } = 80;

    public bool DebugMode { get; set; } = false;

    static DrawingTablet()
    {
        defaulttablet = SKBitmap.Decode(AssetLoader.Open(new Uri("avares://PngifyMeCode/Assets/tablet.png")));
        defaultstylus = SKBitmap.Decode(AssetLoader.Open(new Uri("avares://PngifyMeCode/Assets/stylus.png")));
    }

    public override void OnEnter()
    {
        HotkeyManager.Hook.MouseMoved += OnMouseMove;

        int width;
        if (string.IsNullOrEmpty(TabletPath))
            tablet = new StaticImage(defaulttablet.Copy());
        else
            tablet = BaseImage.LoadFromPath(TabletPath);
        width = tablet.Width;
        tablet.Resize(Specsmanager.Width * Scale / 100, Specsmanager.Height * Scale / 100);

        float scale = tablet.Width / (float)width;

        if (string.IsNullOrEmpty(StylusPath))
            stylus = new StaticImage(defaultstylus.Copy());
        else
            stylus = BaseImage.LoadFromPath(StylusPath);

        stylus.Resize((int)(stylus.Width * scale), (int)(stylus.Height * scale));

        base.OnEnter();
    }

    public override void OnExit()
    {
        HotkeyManager.Hook.MouseMoved -= OnMouseMove;
        base.OnExit();
    }

    private void OnMouseMove(object? sender, MouseHookEventArgs e)
    {
        x = e.RawEvent.Mouse.X;
        y = e.RawEvent.Mouse.Y;
        if (DebugMode && count++ % 60 == 0)
            Log.Information($"Cursor: X {x}/ Y {y}");
    }

    public override void RenderImage(SKCanvas canvas, float offsetX, float offsetY)
    {
        int wiggleXY = 5;
        float sinX = MathF.Sin(CurrentTime) * wiggleXY; // change this to be x/y
        float sinY = MathF.Sin(CurrentTime * 2) * wiggleXY; // change this to be x/y
        float sinRot = MathF.Sin(CurrentTime) * 2; // change this to be x/y

        float scalX = ScaleValue(x, ScreenMinX, ScreenMaxX, 0, Specsmanager.Width);
        float scalY = ScaleValue(y, ScreenMinY, ScreenMaxY, 0, Specsmanager.Height);

        canvas.Save();
        float posx = Math.Clamp(scalX + sinX, 0, Specsmanager.Width);
        float posy = Math.Clamp(scalY + sinY, 0, Specsmanager.Height);
        //posx += -stylus.Width / 2;
        //posy += -stylus.Height;
        canvas.RotateDegrees((posx / Specsmanager.Width - 0.5f) * 2 * 20 + sinRot, posx, posy);
        canvas.DrawBitmap(stylus.GetImage(TimeSpan.FromSeconds(CurrentTime)),
            posx - stylus.Width / 2 + offsetX,
            posy - stylus.Height + offsetY);
        canvas.Restore();

        canvas.DrawBitmap(tablet.GetImage(TimeSpan.FromSeconds(CurrentTime)),
            Specsmanager.Width / 2 - tablet.Width / 2,
            Specsmanager.Height - tablet.Height);
    }

    public static float ScaleValue(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        if (oldMax - oldMin == 0)
            throw new ArgumentException("Old min and max cannot be the same (division by zero).");

        return ((value - oldMin) / (oldMax - oldMin)) * (newMax - newMin) + newMin;
    }

    public override void OnCalculateParameters(float dt, ref LayerValues values)
    {
        //nothing
    }
}