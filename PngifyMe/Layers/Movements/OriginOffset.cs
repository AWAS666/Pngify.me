using PngifyMe.Layers.Helper;
using PngifyMe.Services;
using System;

namespace PngifyMe.Layers
{
    [LayerDescription("Y movement whenever talking")]
    public class OriginOffset : PermaLayer
    {
        [Unit("Pixels")]
        public float OriginX { get; set; } = 0f;

        [Unit("Pixels")]
        public float OriginY { get; set; } = 0f;

        public OriginOffset()
        {
        }
        public override void OnCalculateParameters(float dt, ref LayerValues values)
        {
           values.OriginOffsetX += OriginX;
           values.OriginOffsetY += OriginY;
        }
    }
}
