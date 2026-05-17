using PngifyMe.Layers.Helper;
using PngifyMe.Services;
using SkiaSharp;
using System.Text.Json.Serialization;

namespace PngifyMe.Layers.Image;

[LayerDescription("CropLayer")]
public class CropLayer : ImageLayer
{
    [Unit("bool")]
    public bool InvertedMask { get; set; } = false;

    [Unit("pixels (center offset)")]
    public float CenterOffsetX { get; set; } = 0f;

    [Unit("pixels (center offset)")]
    public float CenterOffsetY { get; set; } = 0f;

    [Unit("pixels")]
    public float Width { get; set; } = 800f;

    [Unit("pixels")]
    public float Height { get; set; } = 800f;

    [JsonIgnore]
    [CanvasRegion]
    public CanvasRect2D Region
    {
        get => new() { X = CenterOffsetX, Y = CenterOffsetY, Width = Width, Height = Height };
        set
        {
            CenterOffsetX = value.X;
            CenterOffsetY = value.Y;
            Width = value.Width;
            Height = value.Height;
        }
    }

    private bool _clipSaved;

    public CropLayer()
    {
        ApplyOtherEffects = true;
        BehindModel = true;
    }

    public override void OnCalculateParameters(float dt, ref LayerValues values)
    {
    }

    public override void RenderImage(SKCanvas canvas, float offsetX, float offsetY)
    {
        if (Width <= 0 || Height <= 0)
            return;

        int width = Specsmanager.Width;
        int height = Specsmanager.Height;
        var clip = GetClipRect(width, height);

        canvas.Save();
        _clipSaved = true;

        if (InvertedMask)
        {
            canvas.ClipRect(SKRect.Create(0, 0, width, height));
            canvas.ClipRect(clip, SKClipOperation.Difference, antialias: true);
        }
        else
        {
            canvas.ClipRect(clip, SKClipOperation.Intersect, antialias: true);
        }
    }

    public override void RenderImageAfterAvatar(SKCanvas canvas, float offsetX, float offsetY)
    {
        if (!_clipSaved)
            return;

        canvas.Restore();
        _clipSaved = false;
    }

    private SKRect GetClipRect(int canvasWidth, int canvasHeight)
    {
        float cx = canvasWidth / 2f + CenterOffsetX;
        float cy = canvasHeight / 2f + CenterOffsetY;
        return SKRect.Create(cx - Width / 2f, cy - Height / 2f, Width, Height);
    }
}
