using PngifyMe.Layers.Helper;
using PngifyMe.Layers.Movements;
using System;

namespace PngifyMe.Layers
{
    public class Yeet : MovementBaseLayer
    {
        [Unit("pixels/s")]
        public float Speed { get; set; } = 4000;

        [Unit("seconds")]
        public float TotalTime { get; set; } = 8;


        public Yeet()
        {
            EnterTime = 2f;
            ExitTime = 2;
        }

        public override void OnEnter()
        {
            AutoRemoveTime = TotalTime;
            base.OnEnter();
        }


        public override void OnCalculateParameters(float dt, ref LayerValues values)
        {
            float speed = CurrentTime * Speed * CurrentStrength;
            float grav = CurrentTime * CurrentTime * CurrentStrength * Speed / (TotalTime / 3 * 2);

            float y = grav - speed;

            if (grav > speed)
            {
                IsExiting = true;
                float factor = Speed / 40;
                y = (float)(Math.Sin(CurrentTime * 10) * factor - factor);
                y *= CurrentStrength;
            }

            values.PosY += y;
        }
    }
}
