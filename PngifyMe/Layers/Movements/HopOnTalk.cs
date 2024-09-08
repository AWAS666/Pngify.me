using PngifyMe.Layers.Helper;
using PngifyMe.Layers.Movements;
using PngifyMe.Services;
using System;

namespace PngifyMe.Layers
{
    public class HopOnTalk : RampOnConditionLayer
    {
        [Unit("pixels")]
        public float Offset { get; set; } = 20f;
        public float Frequency { get; set; } = 5f;
        public HopOnTalk()
        {
        }
        public override void OnCalculateParameters(float dt, ref LayerValues values)
        {
            float pi2 = MathF.PI * 2;
            values.PosY += (float)Math.Sin(pi2 * CurrentTime * Frequency) * Offset * CurrentStrength 
                + Offset * CurrentStrength;
        }

        public override bool Triggered() => AudioService.Talking;
    }
}
