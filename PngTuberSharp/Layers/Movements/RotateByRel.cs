using PngTuberSharp.Layers.Movements;

namespace PngTuberSharp.Layers
{
    public class RotateByRel : MovementBaseLayer
    {
        public float Rotation { get; set; } = 360;
        public float TotalTime { get; set; } = 2f;

        public RotateByRel()
        {
            AutoRemoveTime = TotalTime;
            ExitTime = 0f;
        }
        public override void OnCalculateParameters(float dt, ref LayerValues values)
        {
            values.Rotation += Rotation * CurrentTime / TotalTime;
        }
    }
}
