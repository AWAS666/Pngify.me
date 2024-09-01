using PngifyMe.Layers.Movements;
using PngifyMe.Services;

namespace PngifyMe.Layers
{
    public class SquishOnTalk : MovementBaseLayer
    {
        public float StrengthX { get; set; } = 0.2f;
        public float StrengthY { get; set; } = 0.2f;
        public float Frequency { get; set; } = 5f;
        public SquishOnTalk()
        {
            EnterTime = 0f;
        }
        public override void OnCalculateParameters(float dt, ref LayerValues values)
        {
            if (MicrophoneService.Talking)
            {
                values.ZoomX += StrengthX * CurrentStrength;
                values.ZoomY += StrengthY * CurrentStrength;
            }
        }
    }
}
