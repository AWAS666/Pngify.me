﻿using PngifyMe.Layers.Helper;
using PngifyMe.Layers.Movements;
using System;

namespace PngifyMe.Layers;

[LayerDescription("Wiggle")]
public class Wiggle : MovementBaseLayer
{
    [Unit("pixels")]
    public float Offset { get; set; } = 20f;
    public float Frequency { get; set; } = 5f;

    public override void OnCalculateParameters(float dt, ref LayerValues values)
    {
        float pi2 = MathF.PI * 2;
        values.PosY += (float)Math.Sin(pi2 * GlobalTime * Frequency) * Offset * CurrentStrength;
        values.PosX += (float)Math.Sin(pi2 * GlobalTime * Frequency / 2) * Offset * CurrentStrength;
    }
}
