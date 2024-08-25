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
        private float rotSpeed;

        public Vector2 CurrentSpeed => speed;
        public float X { get => Values.PosX; }
        public float Y { get => Values.PosY; }
        public float Rotation { get => Values.Rotation; }
        public SKBitmap Image { get => Values.Image; }

        public CollisionDetector Collision { get; }


        public MovableObject(SKBitmap map, Vector2 speed, float rotSpeed, int x, int y, int details)
        {
            Values = new LayerValues();
            Values.Image = map;
            Values.PosX = x;
            Values.PosY = y;
            Collision = new CollisionDetector(map, details);
            Collision.Offset = new SKPoint(Values.PosX, Values.PosY);
            this.speed = speed;
            this.rotSpeed = rotSpeed;
        }

        public void Update(float dt)
        {
            Values.PosX += speed.X * dt;
            Values.PosY += speed.Y * dt;
            Values.Rotation += rotSpeed * dt;

            // settle for a gravity constant here
            speed.Y -= -dt * 100;
            Collision.Offset = new SKPoint(Values.PosX, Values.PosY);
        }

        public void SetCollision(float dt)
        {
            // just invert vector I guess
            speed.X = -speed.X * 0.9f;
            speed.Y = -speed.Y * 0.9f;

            // update position to make it move away quicker
            Update(dt);
        }    

        public bool SameBitmap(SKBitmap bitmap)
        {
            return bitmap == Values.Image;
        }
    }
}
