using PngifyMe.Layers.Helper;
using PngifyMe.Layers.Movements;
using System;

namespace PngifyMe.Layers
{
    public class Panic : MovementBaseLayer
    {
        private float zoom;

        //[Unit("pixels/s")]
        public float Speed = 400;

        [Unit("times")]
        public int Cycles { get; set; } = 5;


        public Panic()
        {
            EnterTime = 2f;
            ExitTime = 1f;
        }


        public override void OnCalculateParameters(float dt, ref LayerValues values)
        {
            float sin = (float)Math.Sin(CurrentTime * Speed / 300);
            float x = (float)(-sin * Speed * CurrentStrength * Easings.SineEaseIn(Math.Abs(sin)));
            if (x > Speed * 0.75)
            {
                zoom = Math.Clamp(zoom + dt * 4 * Speed / 400, -2, 0);
            }
            if (x < -Speed * 0.75f)
            {
                zoom = Math.Clamp(zoom - dt * 4 * Speed / 400, -2, 0);
            }


            if (CurrentTime > Cycles * Math.PI)
            {
                IsExiting = true;
            }

            values.ZoomX += zoom;
            values.PosX += x;
            values.PosY += (float)Math.Sin(CurrentTime * Speed / 30) * Speed / 10 * CurrentStrength;
        }
    }
}
