using PngifyMe.Layers.Helper;
using PngifyMe.Layers.Movements;
using PngifyMe.Services;

namespace PngifyMe.Layers
{
    public class SquishOnTalk : RampOnConditionLayer
    {
        [Unit("%")]
        public uint StrengthX { get; set; } = 20;

        [Unit("%")]
        public uint StrengthY { get; set; } = 20;
        public SquishOnTalk()
        {
            EnterTime = 0f;
        }
        public override void OnCalculateParameters(float dt, ref LayerValues values)
        {
            values.ZoomX += StrengthX / 100f * CurrentStrength;
            values.ZoomY += StrengthY / 100f * CurrentStrength;
        }

        public override bool Triggered() => MicrophoneService.Talking;
    }
}
