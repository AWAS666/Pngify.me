﻿using PngTuberSharp.Layers.Helper;
using PngTuberSharp.Layers.Movements;
using System;

namespace PngTuberSharp.Layers
{
    public class GoHome : MovementBaseLayer
    {
        [Unit("pixels/s")]
        public float Speed { get; set; } = 400;

        [Unit("seconds")]
        public float TotalTime { get; set; } = 8;

        private bool Reverse;

        public GoHome()
        {
            EnterTime = 2f;
            ExitTime = 0;
        }


        public override void OnCalculateParameters(float dt, ref LayerValues values)
        {
            values.PosX += CurrentTime * Speed * CurrentStrength;
            values.PosY += (float)Math.Sin(CurrentTime * Speed / 30) * Speed / 10 * CurrentStrength;
        }

        public override bool Update(float dt)
        {
            if (CurrentTime >= TotalTime / 2)
                Reverse = true;

            if (CurrentTime < 0)
                return true;
            return base.Update(Reverse ? -dt : dt);
        }
    }
}
