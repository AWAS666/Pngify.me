namespace PngTuberSharp.Layers.Microphone
{
    public abstract class MicroPhoneLayer
    {

        public abstract void Update(float dt, ref LayerValues values);
        public abstract void Enter();
    }
}