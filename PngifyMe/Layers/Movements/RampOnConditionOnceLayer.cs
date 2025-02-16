using PngifyMe.Layers.Helper;
using System;

namespace PngifyMe.Layers;

public abstract class RampOnConditionOnceLayer : PermaLayer
{
    private float stateChangeTime;
    private bool lastCondition;
    private float activatedTime;
    private bool activating;

    [Unit("s")]
    public float ActivationRamp { get; set; } = 0.5f;

    [Unit("s")]
    public float ActiveTime { get; set; } = 0.5f;

    [Unit("s")]
    public float DeActivationRamp { get; set; } = 0.5f;

    [Unit("s")]
    public float RemoveTime { get; set; } = float.MaxValue;

    public override void OnEnter()
    {
        AutoRemoveTime = RemoveTime;
        base.OnEnter();
    }

    public RampOnConditionOnceLayer()
    {
        EnterTime = 0f;
        ExitTime = 0f;
    }

    public abstract bool Triggered();

    public override void OnUpdate(float dt, float time)
    {
        bool current = Triggered();
        if (!lastCondition && current && !activating)
        {
            if (CurrentTime - stateChangeTime < DeActivationRamp)
            {
                var perce = 1 - (CurrentTime - stateChangeTime) / DeActivationRamp;
                stateChangeTime = CurrentTime - ActivationRamp * perce;
            }
            else
                stateChangeTime = CurrentTime;
            activatedTime = stateChangeTime;
            activating = true;
        }

        if (activatedTime + ActivationRamp + ActiveTime < CurrentTime && activating)
        {
            if (CurrentTime - stateChangeTime < ActivationRamp)
            {
                var perce = (CurrentTime - stateChangeTime) / DeActivationRamp;
                stateChangeTime = CurrentTime - DeActivationRamp * (1 - perce);
            }
            else
                stateChangeTime = CurrentTime;
            activating = false;
        }

        var value = Easings.CubicEaseInOut(Math.Min(1, (CurrentTime - stateChangeTime) / (activating ? ActivationRamp : DeActivationRamp)));

        if (activating)
            CurrentStrength = value;
        else
            CurrentStrength = 1 - value;

        if (stateChangeTime == 0)
            CurrentStrength = 0;

        lastCondition = current;
        base.OnUpdate(dt, time);
    }

}
