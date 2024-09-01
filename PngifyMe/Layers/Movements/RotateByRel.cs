using PngifyMe.Layers.Movements;

namespace PngifyMe.Layers
{
    public class RotateByRel : MovementBaseLayer
    {
        public float Rotation { get; set; } = 360;
        public float TotalTime { get; set; } = 2f;

        public RotateByRel()
        {
            ExitTime = 0f;
            EnterTime = 0f;
        }

        public override void OnEnter()
        {
            AutoRemoveTime = TotalTime;
            base.OnEnter();
        }
        public override void OnCalculateParameters(float dt, ref LayerValues values)
        {
            values.Rotation += Rotation * CurrentTime / TotalTime;
        }
    }
}
