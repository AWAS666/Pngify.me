using PngifyMe.Layers;
using SkiaSharp;
using System;
using System.Numerics;

namespace PngifyMe.Services.ThrowingSystem
{
    public class MovableObject
    {
        private LayerValues Values;

        private Vector2 speed;
        private float rotSpeed;
        private ThrowingSystem parent;

        public DateTime Created { get; }

        public Vector2 CurrentSpeed => speed;
        public float X { get => Values.PosX; }
        public float Y { get => Values.PosY; }
        public float Rotation { get => Values.Rotation; }
        public SKBitmap Image { get => Values.Image; }

        public CollisionDetector Collision { get; }


        public MovableObject(ThrowingSystem parent, SKBitmap item, Vector2 speed, float rotSpeed, int x, int y, int details)
        {
            Values = new LayerValues();
            Values.Image = item;
            Values.PosX = x;
            Values.PosY = y;
            Collision = CollissionCache.GetAndCache(item, details);
            Collision.Offset = new SKPoint(Values.PosX, Values.PosY);
            this.speed = speed;
            this.rotSpeed = rotSpeed;
            this.parent = parent;
            Created = DateTime.Now;
        }

        public void Update(float dt)
        {
            Values.PosX += speed.X * dt;
            Values.PosY += speed.Y * dt;
            Values.Rotation += rotSpeed * dt;

            speed.Y -= -dt * SettingsManager.Current.Tits.Gravity;
            Collision.Offset = new SKPoint(Values.PosX, Values.PosY);
        }

        public void Update(int x, int y)
        {
            Values.PosX = x;
            Values.PosY = y;
            Collision.Offset = new SKPoint(Values.PosX, Values.PosY);
        }

        public void SetCollision(float dt)
        {
            // just invert vector I guess
            speed.X = -speed.X * (1f - SettingsManager.Current.Tits.CollissionEnergyLossPercent / 100f);
            speed.Y = -speed.Y * (1f - SettingsManager.Current.Tits.CollissionEnergyLossPercent / 100f);

            // update position to make it move away quicker
            Update(dt);
        }

        public bool SameBitmap(SKBitmap bitmap)
        {
            return bitmap == Values.Image;
        }
    }
}
