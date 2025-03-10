namespace PngifyMe.Layers.Helper
{
    public abstract class PermaLayer : BaseLayer
    {
        protected float CurrentStrength = 0;
        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnExit()
        {
        }

        public override void OnUpdate(float dt, float time)
        {
        }

        public override void OnUpdateEnter(float dt, float fraction)
        {
        }

        public override void OnUpdateExit(float dt, float fraction)
        {
        }
    }
}
