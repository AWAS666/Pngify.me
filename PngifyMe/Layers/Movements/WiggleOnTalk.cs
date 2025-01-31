using PngifyMe.Layers.Helper;
using PngifyMe.Services;
using System;

namespace PngifyMe.Layers;

[LayerDescription("Wiggle whenever talking (x/y movement)")]
public class WiggleOnTalk : RampOnConditionLayer
{
    [Unit("pixels")]
    public float Offset { get; set; } = 15f;
    public float Frequency { get; set; } = 4f;

    public override void OnCalculateParameters(float dt, ref LayerValues values)
    {
        float pi2 = MathF.PI * 2;
        var sine = (float)Math.Sin(pi2 * CurrentTime * Frequency);
        values.PosY += (float)Math.Sin(pi2 * CurrentTime * Frequency) * Offset * CurrentStrength;
        values.PosX += (float)Math.Sin(pi2 * CurrentTime * Frequency / 2) * Offset * CurrentStrength;
    }

    public override bool Triggered() => AudioService.Talking;

}
