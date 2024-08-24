using Avalonia.Media.Imaging;
using PngTuberSharp.Helpers;
using PngTuberSharp.Layers;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PngTuberSharp.Services.ThrowingSystem
{
    public class MovableObject
    {
        private LayerValues Values;

        private Vector2 speed;

        public Vector2 CurrentSpeed => speed;
        public float X { get => Values.PosX; }
        public float Y { get => Values.PosY; }
        public float Rotation { get => Values.Rotation; }
        public SKBitmap Image { get => Values.Image; }

        public CollisionDetector Collision { get; }


        public MovableObject(SKBitmap map, Vector2 speed, int x, int y, int details)
        {
            Values = new LayerValues();
            Values.Image = map;
            Values.PosX = x;
            Values.PosY = y;
            Collision = new CollisionDetector(map, details);
            Collision.Offset = new SKPoint(Values.PosX, Values.PosY);
            this.speed = speed;
        }

        public void Update(float dt)
        {
            Values.PosX += speed.X * dt;
            Values.PosY += speed.Y * dt;
            // settle for a gravity constant here
            speed.Y -= -dt * 100;
            Collision.Offset = new SKPoint(Values.PosX, Values.PosY);
        }

        public void SetCollision()
        {
            // just invert vector I guess
            speed.X = -speed.X * 0.9f;
            speed.Y = -speed.Y * 0.9f;

            // update position to make it move away quicker
            Update(0.1f);
        }

        //private List<(int x, int y)> GetImageOutline()
        //{
        //    var outline = new List<(int x, int y)>();

        //    var bitmap = Values.Image;

        //    for (int y = 0; y < bitmap.Height; y += 4)
        //    {
        //        for (int x = 0; x < bitmap.Width; x += 4)
        //        {
        //            var color = bitmap.GetPixel(x, y);
        //            if (color.Alpha > 0) // If pixel is not fully transparent
        //            {
        //                outline.Add((x, y));
        //            }
        //        }
        //    }
        //    return outline;
        //}

        public bool SameBitmap(SKBitmap bitmap)
        {
            return bitmap == Values.Image;
        }
    }
}
