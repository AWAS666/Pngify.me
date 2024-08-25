using Avalonia.Platform;
using PngTuberSharp.Layers;
using PngTuberSharp.Services.Twitch;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;

namespace PngTuberSharp.Services.ThrowingSystem
{
    public class ThrowingSystem
    {
        public List<MovableObject> Objects { get; set; } = new();
        public MovableObject MainBody { get; set; }

        public EventHandler<float> UpdateObjects;
        private Vector2 recoil = Vector2.Zero;

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
            TwitchEventSocket.BitsUsed += TriggerBits;
            TwitchEventSocket.RedeemUsed += TriggerRedeem;
        }

        private void TriggerRedeem(object? sender, string e)
        {

        }

        private void TriggerBits(object? sender, ChannelCheer e)
        {
            Trigger(e.Bits);
        }

        public void Update(float dt, ref Layers.LayerValues layert)
        {
            // dampend recoil
            recoil /= 1.5f;

            foreach (var obj in Objects.ToList())
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
                    RecoilMain(dt, obj, ref layert);
                    obj.SetCollision(dt);

                }
                if (obj.X > 2200 || obj.X < -300 || obj.Y > 1300 || obj.Y < -300)
                {
                    Objects.Remove(obj);
                    //obj.SetCollision(dt);
                }
            }

            layert.PosX += recoil.X;
            layert.PosY += recoil.Y;
            layert.Rotation += recoil.Length() / 2;

            UpdateObjects?.Invoke(this, dt);
        }

        private void RecoilMain(float dt, MovableObject? obj, ref LayerValues layert)
        {
            recoil += new Vector2(obj.CurrentSpeed.X * dt, obj.CurrentSpeed.Y * dt);
        }

        public void SwapImage(SKBitmap bitmap, Layers.LayerValues layert)
        {
            if (MainBody != null && MainBody.SameBitmap(bitmap))
                return;
            int posX = (int)((1920 - layert.Image.Width) / 2 + layert.PosX);
            MainBody = new MovableObject(bitmap, new(0, 0), 0, posX, (int)(0 + layert.PosY), 15);
        }

        private bool IsColliding(MovableObject image1, MovableObject image2)
        {
            return image1.Collision.CollidesWith(image2.Collision);
        }

        public void Trigger(int amount)
        {
            for (var i = 0; i < amount; i++)
            {
                Objects.Add(new MovableObject(Throwables.ElementAt(Random.Shared.Next(0, Throwables.Count)),
                    new Vector2(Random.Shared.Next(2000, 2500), -300),
                    Random.Shared.Next(-20, 20)
                    , -100, Random.Shared.Next(400, 700), 10));
            }
        }
    }
}
