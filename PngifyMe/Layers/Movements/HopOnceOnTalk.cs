using PngifyMe.Layers.Helper;
using PngifyMe.Services;
using System;

namespace PngifyMe.Layers;

[LayerDescription("HopOnce")]
public class HopOnceOnTalk : RampOnConditionOnceLayer
{
    private float timeOffset;

    [Unit("pixels")]
    public float Offset { get; set; } = 20f;
    public float Frequency { get; set; } = 2f;
    public HopOnceOnTalk()
    {
    }
    public override void OnCalculateParameters(float dt, ref LayerValues values)
    {
        // this should make it so sinus always start from zero on fresh trigger
        if (Triggering)
            timeOffset = GlobalTime;

        float pi2 = MathF.PI * 2;
        values.PosY += (float)Math.Sin(pi2 * GlobalTime - timeOffset * Frequency)
            * Offset * CurrentStrength
            - Offset * CurrentStrength;
    }

    public override bool Triggered() => AudioService.Talking;
}
