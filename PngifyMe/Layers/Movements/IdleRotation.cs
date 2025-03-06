using PngifyMe.Layers.Helper;
using PngifyMe.Layers.Movements;
using System;

namespace PngifyMe.Layers;

[LayerDescription("IdleRotation")]
public class IdleRotation : MovementBaseLayer
{
    [Unit("degrees")]
    public float Offset { get; set; } = 1f;
    public float Frequency { get; set; } = 0.5f;

    public override void OnCalculateParameters(float dt, ref LayerValues values)
    {
        float pi2 = MathF.PI * 2;
        values.Rotation += (float)Math.Sin(pi2 * GlobalTime * Frequency) * Offset * CurrentStrength;
    }
}
