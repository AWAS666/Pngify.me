using PngifyMe.Layers.Helper;
using SkiaSharp;

namespace PngifyMe.Layers;

public abstract class ImageLayer : PermaLayer
{
    [Unit("bool")]
    public bool ApplyOtherEffects { get; set; } = false;

    [Unit("bool")]
    public bool BehindModel { get; set; } = false;

    public abstract void RenderImage(SKCanvas canvas, float x, float y);

    public ImageLayer()
    {
        ExitTime = 0f;
        EnterTime = 0f;
    }
}

public abstract class BitMapTransformer : PermaLayer
{
    public abstract SKBitmap RenderBitmap(SKBitmap bitmap);

    public BitMapTransformer()
    {
        ExitTime = 0f;
        EnterTime = 0f;
    }
}
