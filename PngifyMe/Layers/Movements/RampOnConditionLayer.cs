using PngifyMe.Layers.Helper;
using System;

namespace PngifyMe.Layers
{
    public abstract class RampOnConditionLayer : PermaLayer
    {
        private float stateChangeTime;
        private bool lastCondition;

        [Unit("s")]
        public float ActivationRamp { get; set; } = 0.5f;

        [Unit("s")]
        public float DeActivationRamp { get; set; } = 0.5f;

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
                stateChangeTime = CurrentTime;
            }
            if (lastCondition && !current)
            {
                stateChangeTime = CurrentTime;
            }

            var value = Math.Min(1,
                Easings.CubicEaseOut((CurrentTime - stateChangeTime) / (current ? ActivationRamp : DeActivationRamp)));

            if (current)
                CurrentStrength = value;
            else
                CurrentStrength = 1 - value;

            lastCondition = current;
            base.OnUpdate(dt, time);
        }

    }
}
