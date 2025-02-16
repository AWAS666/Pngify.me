using PngifyMe.Layers.Helper;
using System;

namespace PngifyMe.Layers;

public abstract class RampOnConditionLayer : PermaLayer
{
    private float stateChangeTime;
    private bool lastCondition;

    [Unit("s")]
    public float ActivationRamp { get; set; } = 0.3f;

    [Unit("s")]
    public float DeActivationRamp { get; set; } = 0.3f;

    [Unit("s")]
    public float RemoveTime { get; set; } = float.MaxValue;

    public override void OnEnter()
    {
        AutoRemoveTime = RemoveTime;
        base.OnEnter();
    }

    public RampOnConditionLayer()
    {
        EnterTime = 0f;
        ExitTime = 0f;
    }

    public abstract bool Triggered();

    public override void OnUpdate(float dt, float time)
    {
        bool current = Triggered();
        if (!lastCondition && current)
        {
            if (CurrentTime - stateChangeTime < DeActivationRamp)
            {
                var perce = 1 - (CurrentTime - stateChangeTime) / DeActivationRamp;
                stateChangeTime = CurrentTime - ActivationRamp * perce;
            }
            else
                stateChangeTime = CurrentTime;

        }
        if (lastCondition && !current)
        {
            if (CurrentTime - stateChangeTime < ActivationRamp)
            {
                var perce = (CurrentTime - stateChangeTime) / DeActivationRamp;
                stateChangeTime = CurrentTime - DeActivationRamp * (1 - perce);
            }
            else
                stateChangeTime = CurrentTime;
        }

        var value = Easings.CubicEaseInOut(Math.Min(1, (CurrentTime - stateChangeTime) / (current ? ActivationRamp : DeActivationRamp)));

        if (current)
            CurrentStrength = value;
        else
            CurrentStrength = 1 - value;

        if (stateChangeTime == 0)
            CurrentStrength = 0;

        lastCondition = current;
        base.OnUpdate(dt, time);
    }

}
