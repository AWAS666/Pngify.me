using PngifyMe.Layers.Helper;
using PngifyMe.Layers.Movements;
using PngifyMe.Services;

namespace PngifyMe.Layers
{
    public class SquishOnTalk : MovementBaseLayer
    {
        [Unit("%")]
        public uint StrengthX { get; set; } = 20;

        [Unit("%")]
        public uint StrengthY { get; set; } = 20;
        public float Frequency { get; set; } = 5f;
        public SquishOnTalk()
        {
            EnterTime = 0f;
        }
        public override void OnCalculateParameters(float dt, ref LayerValues values)
        {
            if (MicrophoneService.Talking)
            {
                values.ZoomX += StrengthX / 100f * CurrentStrength;
                values.ZoomY += StrengthY / 100f * CurrentStrength;
            }
        }
    }
}
