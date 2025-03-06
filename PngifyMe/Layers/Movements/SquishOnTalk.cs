using PngifyMe.Layers.Helper;
using PngifyMe.Services;

namespace PngifyMe.Layers;

[LayerDescription("StretchXY")]
public class SquishOnTalk : RampOnConditionLayer
{
    [Unit("%")]
    public uint StrengthX { get; set; } = 5;

    [Unit("%")]
    public uint StrengthY { get; set; } = 0;
    public SquishOnTalk()
    {
        EnterTime = 0f;
    }
    public override void OnCalculateParameters(float dt, ref LayerValues values)
    {
        values.ZoomX += StrengthX / 100f * CurrentStrength;
        values.ZoomY += StrengthY / 100f * CurrentStrength;
    }

    public override bool Triggered() => AudioService.Talking;
}
