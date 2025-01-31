using PngifyMe.Layers.Helper;
using PngifyMe.Services;
using System;

namespace PngifyMe.Layers;

[LayerDescription("X/Y movement all the time")]
public class IdleMove : PermaLayer
{
    [Unit("pixels")]
    public float OffsetX { get; set; } = 0;

    [Unit("pixels")]
    public float OffsetY { get; set; } = 20f;
    public float Frequency { get; set; } = 0.2f;
    public IdleMove()
    {
    }
    public override void OnCalculateParameters(float dt, ref LayerValues values)
    {
        float pi2 = MathF.PI * 2;
        float freq = (float)Math.Sin(pi2 * CurrentTime * Frequency);
        values.PosY += freq * OffsetY;
        values.PosX += freq * OffsetX;
    }
}
