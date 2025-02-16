using PngifyMe.Layers.Helper;

namespace PngifyMe.Layers.Movements;

public abstract class MovementBaseLayer : BaseLayer
{
    protected float CurrentStrength = 0;
    public override void OnCalculateParameters(float dt, ref LayerValues values)
    {

    }

    public override void OnEnter()
    {

    }

    public override void OnExit()
    {

    }

    public override void OnUpdate(float dt, float time)
    {
        CurrentStrength = 1.0f;

    }

    public override void OnUpdateEnter(float dt, float fraction)
    {
        CurrentStrength = Easings.CubicEaseOut(fraction);
    }

    public override void OnUpdateExit(float dt, float fraction)
    {
        CurrentStrength = 1.0f - Easings.CubicEaseOut(fraction);
    }
}
