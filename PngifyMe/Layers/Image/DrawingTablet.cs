using Avalonia.Platform;
using PngifyMe.Helpers;
using PngifyMe.Layers.Helper;
using PngifyMe.Services;
using PngifyMe.Services.Hotkey;
using SharpHook;
using SkiaSharp;
using System;

namespace PngifyMe.Layers.Image;

[LayerDescription("DrawingTablet")]
public class DrawingTablet : ImageLayer
{
    private static SKBitmap defaulttablet;
    private static float defaultScale;
    private static SKBitmap defaultstylus;
    private SKBitmap tablet;
    private SKBitmap stylus;
    private short x;
    private short y;

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

    static DrawingTablet()
    {
        defaulttablet = SKBitmap.Decode(AssetLoader.Open(new Uri("avares://PngifyMeCode/Assets/tablet.png")));
        var width = defaulttablet.Width;
        var height = defaulttablet.Height;
        defaulttablet = SkiaHelper.Resize(defaulttablet, (int)(Specsmanager.Width * 0.8f), (int)(Specsmanager.Height * 0.8f));
        defaultScale = defaulttablet.Width / (float)width;

        defaultstylus = SKBitmap.Decode(AssetLoader.Open(new Uri("avares://PngifyMeCode/Assets/stylus.png")));
        defaultstylus = SkiaHelper.Resize(defaultstylus, defaultstylus.Width * defaulttablet.Width / width, defaultstylus.Height * height / defaulttablet.Height);

        defaulttablet.SetImmutable();
        defaultstylus.SetImmutable();
    }

    public override void OnEnter()
    {
        // todo:
        // rescale based on parameter
        HotkeyManager.MouseHook.MouseMoved += OnMouseMove;
        if (!HotkeyManager.MouseHook.IsRunning)
            HotkeyManager.MouseHook.RunAsync();

        float scale = defaultScale;
        if (string.IsNullOrEmpty(TabletPath))
        {
            tablet = defaulttablet;
        }
        else
        {
            // load and scale external
            tablet = SKBitmap.Decode(TabletPath);
            var width = tablet.Width;
            tablet = SkiaHelper.Resize(tablet, (int)(Specsmanager.Width * 0.8f), (int)(Specsmanager.Height * 0.8f));
            scale = tablet.Width / (float)width;
        }

        if (string.IsNullOrEmpty(StylusPath))
        {
            stylus = defaultstylus;
        }
        else
        {
            // load and scale external
            stylus = SKBitmap.Decode(StylusPath);
            // scale with tablet
            stylus = SkiaHelper.Resize(stylus, (int)(stylus.Width * scale), (int)(stylus.Height * scale));
        }
        base.OnEnter();
    }

    public override void OnExit()
    {
        HotkeyManager.MouseHook.MouseMoved -= OnMouseMove;
        base.OnExit();
    }

    private void OnMouseMove(object? sender, MouseHookEventArgs e)
    {
        x = e.RawEvent.Mouse.X;
        y = e.RawEvent.Mouse.Y;
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
        canvas.DrawBitmap(stylus, posx - stylus.Width / 2 + offsetX, posy - stylus.Height + offsetY);
        canvas.Restore();

        canvas.DrawBitmap(tablet, Specsmanager.Width / 2 - tablet.Width / 2, Specsmanager.Height - tablet.Height);
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