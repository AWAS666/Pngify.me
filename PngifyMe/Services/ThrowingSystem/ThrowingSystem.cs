using Avalonia.Platform;
using NAudio.Wave;
using PngifyMe.Layers;
using PngifyMe.Services.Twitch;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;

namespace PngifyMe.Services.ThrowingSystem
{
    public class ThrowingSystem
    {
        public List<MovableObject> Objects { get; set; } = new();
        public MovableObject MainBody { get; set; }

        public EventHandler<float> UpdateObjects;
        private Vector2 recoil = Vector2.Zero;
        private Vector2 recoilChange = Vector2.Zero;

        private string cachedSound;
        private Stream audio;

        public List<SKBitmap> Throwables { get; set; } = new()
        {
            SKBitmap.Decode(AssetLoader.Open(new Uri("avares://PngifyMe/Assets/bit1.png"))).Resize(new SKSizeI(50, 50), SKFilterQuality.Medium),
            SKBitmap.Decode(AssetLoader.Open(new Uri("avares://PngifyMe/Assets/bit100.png"))).Resize(new SKSizeI(50, 50), SKFilterQuality.Medium),
            SKBitmap.Decode(AssetLoader.Open(new Uri("avares://PngifyMe/Assets/bit1000.png"))).Resize(new SKSizeI(50, 50), SKFilterQuality.Medium),
            SKBitmap.Decode(AssetLoader.Open(new Uri("avares://PngifyMe/Assets/bit5000.png"))).Resize(new SKSizeI(50, 50), SKFilterQuality.Medium),
            SKBitmap.Decode(AssetLoader.Open(new Uri("avares://PngifyMe/Assets/bit10000.png"))).Resize(new SKSizeI(50, 50), SKFilterQuality.Medium),
            SKBitmap.Decode(AssetLoader.Open(new Uri("avares://PngifyMe/Assets/bit20000.png"))).Resize(new SKSizeI(50, 50), SKFilterQuality.Medium),
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
            // dont do e.Bits here as that may (tm) crash it
            Trigger(Math.Clamp(e.Bits / 10, 5, 50));
        }

        public void Update(float dt, ref Layers.LayerValues layert)
        {
            // dampend recoil
            recoilChange /= 1 + (dt * 4f);
            recoilChange -= dt * recoil / 2f;

            foreach (var obj in Objects.ToList())
            {
                obj.Update(dt);
                if (IsColliding(obj, MainBody))
                {
                    RecoilMain(dt, obj, ref layert);
                    obj.SetCollision(dt);
                    if (SettingsManager.Current.Tits.EnableSound)
                        PlaySound(obj);

                }
                if (obj.X > Specsmanager.Width + 200 || obj.X < -200 || obj.Y > Specsmanager.Height + 200 || obj.Y < -200
                    // remove by age
                    || obj.Created.AddSeconds(30) < DateTime.Now)
                {
                    Objects.Remove(obj);
                }
            }

            recoil += recoilChange;

            layert.PosX += recoil.X / 4;
            layert.PosY += recoil.Y / 4;
            layert.Rotation += recoil.Length() / 5;

            UpdateObjects?.Invoke(this, dt);
        }

        private void PlaySound(MovableObject obj)
        {
            var settings = SettingsManager.Current.Tits;

            if (cachedSound != settings.HitSound || audio == null)
                if (string.IsNullOrEmpty(settings.HitSound))
                    audio = AssetLoader.Open(new Uri("avares://PngifyMe/Assets/oof.wav"));
                else
                    audio = File.OpenRead(settings.HitSound);

            cachedSound = settings.HitSound;

            MemoryStream copy = new MemoryStream();
            audio.CopyTo(copy);
            audio.Seek(0, SeekOrigin.Begin);
            copy.Seek(0, SeekOrigin.Begin);

            var reader = new WaveFileReader(copy);
            var player = new WaveOutEvent();

            player.Init(reader);

            // Subscribe to the PlaybackStopped event to dispose of resources when playback is done
            player.PlaybackStopped += (sender, args) =>
            {
                player.Dispose();
                reader.Dispose();
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
            int posX = (int)((Specsmanager.Width - layert.Image.Width) / 2 + layert.PosX);
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
            int height = Specsmanager.Height / 4;
            int offset = Specsmanager.Height / 8;
            for (var i = 0; i < amount; i++)
            {
                Objects.Add(new MovableObject(
                    this,
                    item: Throwables.ElementAt(Random.Shared.Next(0, Throwables.Count)),
                   speed: new Vector2(Random.Shared.Next((int)SettingsManager.Current.Tits.ObjectSpeedMin, (int)SettingsManager.Current.Tits.ObjectSpeedMax), -300),
                   rotSpeed: Random.Shared.Next(-20, 20)
                    , x: -100, y: Random.Shared.Next(offset, height), details: 10));
            }
        }
    }
}
