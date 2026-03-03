using PngifyMe.Layers.Helper;
using PngifyMe.Services;
using PngifyMe.Services.CharacterSetup.Images;
using Semi.Avalonia.Tokens.Palette;
using Serilog;
using SkiaSharp;
using System;
using System.IO;
using System.Text.Json.Serialization;

namespace PngifyMe.Layers.Image;

public enum TransitionType
{
    FadeInOut,
    FadeIn,
    FadeOut,
    MoveX,
    MoveY,
    MoveXMirror,
    MoveYMirror
}

[LayerDescription("PinImageToModel")]
public class Image : ImageLayer
{
    [Unit("pixels (center)")]
    public float PosX { get; set; } = 960;

    [Unit("pixels (center)")]
    public float PosY { get; set; } = 540;

    [JsonIgnore]
    [CanvasPosition]
    public CanvasPosition2D Position
    {
        get => new() { X = PosX, Y = PosY };
        set { PosX = value.X; PosY = value.Y; }
    }

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
            canvas.DrawBitmap(img, Position.X - img.Width / 2, Position.Y - img.Height / 2);
            return;
        }

        float opacity = 1.0f;
        float renderX = Position.X;
        float renderY = Position.Y;

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
            case TransitionType.MoveXMirror:
                var canvasWidth = Specsmanager.Width;
                var startX = -img.Width / 2;
                var endX = canvasWidth + img.Width / 2;

                if (IsExiting)
                {
                    var exitProgress = CurrentExitingTime / TransitionTime;
                    exitProgress = MathF.Min(exitProgress, 1f);
                    if (TransitionType == TransitionType.MoveX)
                        renderX = Position.X + (endX - Position.X) * exitProgress;
                    else
                        renderX = Position.X - (endX - Position.X) * exitProgress;
                }
                else
                {
                    var enterProgress = CurrentTime / TransitionTime;
                    enterProgress = MathF.Min(enterProgress, 1f);
                    renderX = startX + (Position.X - startX) * enterProgress;
                }
                break;

            case TransitionType.MoveY:
            case TransitionType.MoveYMirror:
                var canvasHeight = Specsmanager.Height;
                var startY = -img.Height / 2;
                var endY = canvasHeight + img.Height / 2;

                if (IsExiting)
                {
                    var exitProgress = CurrentExitingTime / TransitionTime;
                    exitProgress = MathF.Min(exitProgress, 1f);
                    if (TransitionType == TransitionType.MoveY)
                        renderY = Position.Y + (endY - Position.Y) * exitProgress;
                    else
                        renderY = Position.Y - (endY - Position.Y) * exitProgress;
                }
                else
                {
                    var enterProgress = CurrentTime / TransitionTime;
                    enterProgress = MathF.Min(enterProgress, 1f);
                    renderY = startY + (Position.Y - startY) * enterProgress;
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
