using Avalonia.Platform;
using NAudio.Wave;
using PngTuberSharp.Layers;
using PngTuberSharp.Services.Twitch;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;

namespace PngTuberSharp.Services.ThrowingSystem
{
    public class ThrowingSystem
    {
        public List<MovableObject> Objects { get; set; } = new();
        public MovableObject MainBody { get; set; }

        public EventHandler<float> UpdateObjects;
        private Vector2 recoil = Vector2.Zero;
        private Vector2 recoilChange = Vector2.Zero;

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
            recoilChange /= 1 + (dt * 4f);
            recoilChange -= dt * recoil / 2f;

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
                    if (SettingsManager.Current.Tits.EnableSound)
                        PlaySound();

                }
                if (obj.X > 2200 || obj.X < -300 || obj.Y > 1300 || obj.Y < -300)
                {
                    Objects.Remove(obj);
                    //obj.SetCollision(dt);
                }
            }

            recoil += recoilChange;

            layert.PosX += recoil.X / 4;
            layert.PosY += recoil.Y / 4;
            layert.Rotation += recoil.Length() / 5;

            UpdateObjects?.Invoke(this, dt);
        }

        private void PlaySound()
        {
            var settings = SettingsManager.Current.Tits;
            Stream audio = null;
            if (string.IsNullOrEmpty(settings.HitSound))
                audio = AssetLoader.Open(new Uri("avares://PngTuberSharp/Assets/oof.wav"));
            else
                audio = File.OpenRead(settings.HitSound);
            var reader = new WaveFileReader(audio);
            var player = new WaveOutEvent();

            player.Init(reader);

            // Subscribe to the PlaybackStopped event to dispose of resources when playback is done
            player.PlaybackStopped += (sender, args) =>
            {
                player.Dispose();
                reader.Dispose();
                audio.Dispose();
            };
            _ = Task.Run(async () =>
            {
                await Task.Delay(Random.Shared.Next(1, 200));
                player.Play();
            });

        }

        private void RecoilMain(float dt, MovableObject? obj, ref LayerValues layert)
        {
            //recoil += new Vector2(obj.CurrentSpeed.X * dt, obj.CurrentSpeed.Y * dt);
            recoilChange += new Vector2(obj.CurrentSpeed.X * dt / 10, obj.CurrentSpeed.Y * dt / 10);
        }

        public void SwapImage(SKBitmap bitmap, Layers.LayerValues layert)
        {
            int posX = (int)((1920 - layert.Image.Width) / 2 + layert.PosX);
            if (MainBody != null && MainBody.SameBitmap(bitmap))
            {
                MainBody.Update(posX, (int)layert.PosY);
                return;
            }
            MainBody = new MovableObject(this, bitmap, new(0, 0), 0, posX, (int)(0 + layert.PosY), 15);
        }

        private bool IsColliding(MovableObject image1, MovableObject image2)
        {
            return image1.Collision.CollidesWith(image2.Collision);
        }

        public void Trigger(int amount)
        {
            for (var i = 0; i < amount; i++)
            {
                Objects.Add(new MovableObject(
                    this,
                    item: Throwables.ElementAt(Random.Shared.Next(0, Throwables.Count)),
                   speed: new Vector2(Random.Shared.Next((int)SettingsManager.Current.Tits.ObjectSpeedMin, (int)SettingsManager.Current.Tits.ObjectSpeedMax), -300),
                   rotSpeed: Random.Shared.Next(-20, 20)
                    , x: -100, y: Random.Shared.Next(300, 1000), details: 10));
            }
        }
    }
}
