using Avalonia.Platform;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PngTuberSharp.Services.ThrowingSystem
{
    public class ThrowingSystem
    {
        public List<MovableObject> Objects { get; set; } = new();
        public MovableObject MainBody { get; set; }

        public EventHandler<float> UpdateObjects;

        public List<SKBitmap> Throwables { get; set; } = new()
        {
            SKBitmap.Decode(AssetLoader.Open(new Uri("avares://PngTuberSharp/Assets/bit1.png"))).Resize(new SKSizeI(50, 50), SKFilterQuality.Medium),
            SKBitmap.Decode(AssetLoader.Open(new Uri("avares://PngTuberSharp/Assets/bit100.png"))).Resize(new SKSizeI(50, 50), SKFilterQuality.Medium),
            SKBitmap.Decode(AssetLoader.Open(new Uri("avares://PngTuberSharp/Assets/bit1000.png"))).Resize(new SKSizeI(50, 50), SKFilterQuality.Medium),
            SKBitmap.Decode(AssetLoader.Open(new Uri("avares://PngTuberSharp/Assets/bit5000.png"))).Resize(new SKSizeI(50, 50), SKFilterQuality.Medium),
            SKBitmap.Decode(AssetLoader.Open(new Uri("avares://PngTuberSharp/Assets/bit10000.png"))).Resize(new SKSizeI(50, 50), SKFilterQuality.Medium),
            SKBitmap.Decode(AssetLoader.Open(new Uri("avares://PngTuberSharp/Assets/bit20000.png"))).Resize(new SKSizeI(50, 50), SKFilterQuality.Medium),
        };

        public ThrowingSystem()
        {
            for (var i = 0; i < 5; i++)
            {
                Objects.Add(new MovableObject(Throwables.ElementAt(Random.Shared.Next(0, Throwables.Count)),
                    new System.Numerics.Vector2(1000, -300), 0, Random.Shared.Next(200, 800), 15));
            }
        }


        public void Update(float dt, ref Layers.LayerValues layert)
        {
            foreach (var obj in Objects)
            {
                obj.Update(dt);
                //foreach (var otherObject in Objects)
                //{
                //    if (otherObject != obj && IsColliding(obj, otherObject))
                //    {
                //        obj.SetCollision();
                //    }
                //}
                if (IsColliding(obj, MainBody))
                {
                    layert.PosX += obj.CurrentSpeed.X * dt;
                    layert.PosY += obj.CurrentSpeed.Y * dt;
                    obj.SetCollision(dt);

                }
                if (obj.X > 1920 || obj.X < 0 || obj.Y > 1080 || obj.Y < 0)
                    obj.SetCollision(dt);
            }


            UpdateObjects?.Invoke(this, dt);
        }

        public void SwapImage(SKBitmap bitmap, Layers.LayerValues layert)
        {
            if (MainBody != null && MainBody.SameBitmap(bitmap))
                return;
            MainBody = new MovableObject(bitmap, new(0, 0), (int)(540 + layert.PosX), (int)(0 + layert.PosY), 15);
        }

        private bool IsColliding(MovableObject image1, MovableObject image2)
        {
            return image1.Collision.CollidesWith(image2.Collision);
        }

        //private bool IsColliding(MovableObject image1, MovableObject image2)
        //{
        //    foreach (var (x1, y1) in image1.Outline)
        //    {
        //        int globalX1 = (int)(x1 + image1.X);
        //        int globalY1 = (int)(y1 + image1.Y);

        //        foreach (var (x2, y2) in image2.Outline)
        //        {
        //            int globalX2 = (int)(x2 + image2.X);
        //            int globalY2 = (int)(y2 + image2.Y);

        //            if (globalX1 == globalX2 && globalY1 == globalY2)
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

        //private void CollideWithMain(float dt, ref Layers.LayerValues layert)
        //{
        //    var toCheck = Objects.ToList();
        //    foreach (var (x1, y1) in MainBody.Outline)
        //    {
        //        int globalX1 = (int)(x1 + MainBody.X);
        //        int globalY1 = (int)(y1 + MainBody.Y);

        //        foreach (var item in toCheck.ToList())
        //        {
        //            foreach (var (x2, y2) in item.Outline)
        //            {
        //                int globalX2 = (int)(x2 + item.X);
        //                int globalY2 = (int)(y2 + item.Y);

        //                if (globalX1 == globalX2 && globalY1 == globalY2)
        //                {

        //                    layert.PosX += item.CurrentSpeed.X * dt;
        //                    layert.PosY += item.CurrentSpeed.Y * dt;
        //                    item.SetCollision();

        //                    toCheck.Remove(item);
        //                    break;
        //                }
        //            }
        //        }               
        //    }
        //}
    }
}
