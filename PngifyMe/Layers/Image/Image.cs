using PngifyMe.Layers.Helper;
using PngifyMe.Services;
using PngifyMe.Services.CharacterSetup.Images;
using Semi.Avalonia.Tokens.Palette;
using Serilog;
using SkiaSharp;
using System;
using System.IO;

namespace PngifyMe.Layers.Image;

public enum TransitionType
{
    FadeInOut,
    FadeIn,
    FadeOut,
    MoveX,
    MoveY,
}

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

    [Enum]
    public TransitionType TransitionType { get; set; } = TransitionType.FadeInOut;

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
        // do not set this for just fade in, else image will stick for transition time
        if (TransitionType != TransitionType.FadeIn)
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

        var img = GetImage();

        if (TransitionTime == 0f)
        {
            canvas.DrawBitmap(img, PosX - img.Width / 2, PosY - img.Height / 2);
            return;
        }

        float opacity = 1.0f;
        float renderX = PosX;
        float renderY = PosY;

        switch (TransitionType)
        {
            case TransitionType.FadeInOut:
                if (IsExiting)
                {
                    opacity = 1 - CurrentExitingTime / TransitionTime;
                }
                else
                {
                    opacity = CurrentTime / TransitionTime;
                    opacity = MathF.Min(opacity, 1f);
                }
                break;

            case TransitionType.FadeIn:
                if (IsExiting)
                {
                    opacity = 1.0f;
                }
                else
                {
                    opacity = CurrentTime / TransitionTime;
                    opacity = MathF.Min(opacity, 1f);
                }
                break;

            case TransitionType.FadeOut:
                if (IsExiting)
                {
                    opacity = 1 - CurrentExitingTime / TransitionTime;
                }
                else
                {
                    opacity = 1.0f;
                }
                break;

            case TransitionType.MoveX:
                var canvasWidth = Specsmanager.Width;
                var startX = -img.Width / 2;
                var endX = canvasWidth + img.Width / 2;

                if (IsExiting)
                {
                    var exitProgress = CurrentExitingTime / TransitionTime;
                    exitProgress = MathF.Min(exitProgress, 1f);
                    renderX = PosX + (endX - PosX) * exitProgress;
                }
                else
                {
                    var enterProgress = CurrentTime / TransitionTime;
                    enterProgress = MathF.Min(enterProgress, 1f);
                    renderX = startX + (PosX - startX) * enterProgress;
                }
                break;

            case TransitionType.MoveY:
                var canvasHeight = Specsmanager.Height;
                var startY = -img.Height / 2;
                var endY = canvasHeight + img.Height / 2;

                if (IsExiting)
                {
                    var exitProgress = CurrentExitingTime / TransitionTime;
                    exitProgress = MathF.Min(exitProgress, 1f);
                    renderY = PosY + (endY - PosY) * exitProgress;
                }
                else
                {
                    var enterProgress = CurrentTime / TransitionTime;
                    enterProgress = MathF.Min(enterProgress, 1f);
                    renderY = startY + (PosY - startY) * enterProgress;
                }
                break;
        }

        using var paint = new SKPaint { Color = SKColors.White.WithAlpha((byte)(opacity * 255)) };
        canvas.DrawBitmap(img, renderX - img.Width / 2, renderY - img.Height / 2, paint);
    }

    public SKBitmap GetImage()
    {
        return image.GetImage(TimeSpan.FromSeconds(CurrentTime));
    }
}
