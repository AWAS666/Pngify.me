using System;
using System.Collections.Generic;
using System.Drawing;

namespace PngTuberSharp.Services.ThrowingSystem
{
    public class ThrowingSystem
    {
        public List<MovableObject> Objects { get; set; } = new();
        public MovableObject MainBody { get; set; }

        public EventHandler<float> UpdateObjects;

        public void Update(float dt, ref Layers.LayerValues layert)
        {
            foreach (var obj in Objects)
            {
                obj.Update(dt);
                foreach (var otherObject in Objects)
                {
                    if (otherObject != obj && IsColliding(obj, otherObject))
                    {
                        obj.SetCollision();
                    }
                }
                if (IsColliding(obj, MainBody))
                {
                    layert.PosX += obj.CurrentSpeed.X * dt;
                    layert.PosY += obj.CurrentSpeed.Y * dt;
                    obj.SetCollision();

                }
            }

            UpdateObjects?.Invoke(this, dt);
        }

        public void SwapImage(Avalonia.Media.Imaging.Bitmap bitmap)
        {
            if (MainBody != null && MainBody.SameBitmap(bitmap))
                return;
            MainBody = new MovableObject(bitmap, new(0, 0));
        }

        private bool IsColliding(MovableObject image1, MovableObject image2)
        {
            foreach (var (x1, y1) in image1.Outline)
            {
                int globalX1 = (int)(x1 + image1.X);
                int globalY1 = (int)(y1 + image1.Y);

                foreach (var (x2, y2) in image2.Outline)
                {
                    int globalX2 = (int)(x2 + image2.X);
                    int globalY2 = (int)(y2 + image2.Y);

                    if (globalX1 == globalX2 && globalY1 == globalY2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
