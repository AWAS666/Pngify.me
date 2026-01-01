using PngifyMe.Layers.Helper;
using PngifyMe.Services.CharacterSetup.Images;
using Semi.Avalonia.Tokens.Palette;
using Serilog;
using SkiaSharp;
using System;
using System.IO;

namespace PngifyMe.Layers.Image;

[LayerDescription("PinImageToModel")]
public class Image : ImageLayer
{
    [Unit("pixels (center)")]
    public float PosX { get; set; } = 960;

    [Unit("pixels (center)")]
    public float PosY { get; set; } = 540;

    [Unit("Path")]
    [ImagePicker]
    public string FilePath { get; set; } = string.Empty;

    [Unit("seconds")]
    public float StickyFor { get; set; } = float.MaxValue;

    [Unit("seconds")]
    public float TransitionTime { get; set; } = 0.0f;

    private BaseImage image;

    public override void OnEnter()
    {
        if (!File.Exists(FilePath))
        {
            Log.Error($"Image: File {FilePath} not found");
            IsExiting = true;
            return;
        }

        AutoRemoveTime = StickyFor;
        ExitTime = TransitionTime;
        image = BaseImage.LoadFromPath(FilePath);
        base.OnEnter();
    }

    public override void OnExit()
    {
        image?.Dispose();
        base.OnExit();
    }

    public override void OnCalculateParameters(float dt, ref LayerValues values)
    {
        // nothing prolly
    }

    public override void RenderImage(SKCanvas canvas, float x, float y)
    {
        if (image == null)
            return;

        float opacity;
        if (IsExiting)
        {
            opacity = TransitionTime == 0f ? 0f : 1 - CurrentExitingTime / TransitionTime;
        }
        else
        {
            opacity = TransitionTime == 0f ? 1f : CurrentTime / TransitionTime;
            opacity = MathF.Min(opacity, 1f);
        }

        using var paint = new SKPaint { Color = SKColors.White.WithAlpha((byte)(opacity * 255)) };

        var img = GetImage();
        canvas.DrawBitmap(img, PosX - img.Width / 2, PosY - img.Height / 2, paint);
    }

    public SKBitmap GetImage()
    {
        return image.GetImage(TimeSpan.FromSeconds(CurrentTime));
    }
}
