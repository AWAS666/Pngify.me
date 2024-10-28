using Avalonia.Platform;
using PngifyMe.Layers;
using PngifyMe.Services.Settings;
using PngifyMe.Services.Twitch;
using Serilog;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Threading;
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
        private TitsSettings settings;

        public List<SKBitmap> Throwables { get; set; }

        private int _activeThreads = 0;

        public int ActiveThreads => _activeThreads;

        public ThrowingSystem()
        {
            TwitchEventSocket.BitsUsed += TriggerBits;
            TwitchEventSocket.RedeemUsed += TriggerRedeem;
            settings = SettingsManager.Current.Tits;
            settings.UseTwitchEmotesChanged += ReloadEmotes;
            ReloadEmotes(this, settings.UseTwitchEmotes);
        }

        private void ReloadEmotes(object? sender, bool useTwitch)
        {
            if (useTwitch)
            {
                Throwables = new List<SKBitmap>();
                if (TwitchEventSocket.Api != null)
                {
                    LoadTwitchEmotesAsync(null, null);
                }
                else
                {
                    TwitchEventSocket.Authenticated += LoadTwitchEmotesAsync;
                }
            }
            else
            {
                Throwables = LoadBits();
            }
        }

        private List<SKBitmap> LoadBits()
        {
            return
            [
                SKBitmap.Decode(AssetLoader.Open(new Uri("avares://PngifyMe/Assets/bit1.png"))).Resize(new SKSizeI(50, 50), SKFilterQuality.Medium),
                SKBitmap.Decode(AssetLoader.Open(new Uri("avares://PngifyMe/Assets/bit100.png"))).Resize(new SKSizeI(50, 50), SKFilterQuality.Medium),
                SKBitmap.Decode(AssetLoader.Open(new Uri("avares://PngifyMe/Assets/bit1000.png"))).Resize(new SKSizeI(50, 50), SKFilterQuality.Medium),
                SKBitmap.Decode(AssetLoader.Open(new Uri("avares://PngifyMe/Assets/bit5000.png"))).Resize(new SKSizeI(50, 50), SKFilterQuality.Medium),
                SKBitmap.Decode(AssetLoader.Open(new Uri("avares://PngifyMe/Assets/bit10000.png"))).Resize(new SKSizeI(50, 50), SKFilterQuality.Medium),
                SKBitmap.Decode(AssetLoader.Open(new Uri("avares://PngifyMe/Assets/bit20000.png"))).Resize(new SKSizeI(50, 50), SKFilterQuality.Medium),
            ];
        }

        private async void LoadTwitchEmotesAsync(object? sender, TwitchAuth e)
        {
            Throwables.Clear();
            var emotes = await TwitchEventSocket.Api.GetEmotes();
            using var httpClient = new HttpClient();
            string baseDir = Path.Combine(Specsmanager.BasePath, "emotecache");
            Directory.CreateDirectory(baseDir);
            foreach (var emote in emotes)
            {
                string fileName = Path.Combine(baseDir, $"{emote.Name}.png");
                if (File.Exists(fileName))
                {
                    var imageBytes = await File.ReadAllBytesAsync(fileName);
                    using var stream = new SKMemoryStream(imageBytes);
                    Throwables.Add(SKBitmap.Decode(stream).Resize(new SKSizeI(50, 50), SKFilterQuality.Medium));
                }
                else
                {
                    var imageBytes = await httpClient.GetByteArrayAsync(emote.Images.Url4X);
                    await File.WriteAllBytesAsync(fileName, imageBytes);
                    using var stream = new SKMemoryStream(imageBytes);
                    Throwables.Add(SKBitmap.Decode(stream).Resize(new SKSizeI(50, 50), SKFilterQuality.Medium));
                }
            }
        }

        private async void TriggerRedeem(object? sender, string e)
        {

            if (e.Equals(settings.ThrowSetup.Redeem, StringComparison.CurrentCultureIgnoreCase))
                Trigger(20);

            if (e.Equals(settings.RainSetup.Redeem, StringComparison.CurrentCultureIgnoreCase))
            {
                for (var i = 0; i < 10; i++)
                {
                    Rain(10);
                    await Task.Delay(500);
                }
            }
        }

        private async void TriggerBits(object? sender, ChannelCheer e)
        {
            // dont do e.Bits here as that may (tm) crash it
            if (e.Bits > settings.ThrowSetup.MinBits && e.Bits < settings.ThrowSetup.MaxBits)
                Trigger(Math.Clamp(e.Bits / 10, 5, 50));

            if (e.Bits > settings.RainSetup.MinBits && e.Bits < settings.RainSetup.MaxBits)
            {
                for (var i = 0; i < Math.Clamp(e.Bits / 20, 5, 10); i++)
                {
                    Rain(Math.Clamp(e.Bits / 10, 10, 50));
                    await Task.Delay(500);
                }
            }
        }

        public void Update(float dt, ref Layers.LayerValues layert)
        {
            // dampend recoil
            recoilChange /= 1 + (dt * 4f);
            recoilChange -= dt * recoil / 2f;
            var list = Objects.ToList();
            foreach (var obj in list)
            {
                obj.Update(dt);
                if (IsColliding(obj, MainBody))
                {
                    RecoilMain(dt, obj, ref layert);
                    obj.SetCollision(dt);
                    if (settings.EnableSound)
                        PlaySound(obj);

                }
                if (obj.X > Specsmanager.Width + 200 || obj.X < -200 || obj.Y > Specsmanager.Height + 200 || obj.Y < -200
                    // remove by age
                    || obj.Created.AddSeconds(15) < DateTime.Now)
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
            try
            {
                if (obj.AudioPlaying || _activeThreads > 10) return;
                obj.AudioPlaying = true;
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

                _ = Task.Run(async () =>
                {
                    Interlocked.Increment(ref _activeThreads);
                    await AudioService.PlaySoundWav(copy, settings.Volume, true);
                    copy.Dispose();
                    obj.AudioPlaying = false;
                    Interlocked.Decrement(ref _activeThreads);
                });
            }
            catch (Exception e)
            {
                Log.Error("Audio error in tits: " + e.Message, e);
            }

        }

        private void RecoilMain(float dt, MovableObject? obj, ref LayerValues layert)
        {
            //recoil += new Vector2(obj.CurrentSpeed.X * dt, obj.CurrentSpeed.Y * dt);
            recoilChange += new Vector2(obj.CurrentSpeed.X * dt / 10, obj.CurrentSpeed.Y * dt / 10);
        }

        public void SwapImage(SKBitmap bitmap, Layers.LayerValues layert)
        {
            int posX = (int)((Specsmanager.Width - layert.Image.Width) / 2 + layert.PosX);
            int posY = (int)(layert.PosY + Specsmanager.Height * 0.05f); // add 5% here as image is scaled to 90%, so this needs to be 10/2
            if (MainBody != null && MainBody.SameBitmap(bitmap))
            {
                MainBody.Update(posX, posY);
                return;
            }
            MainBody = new MovableObject(this, bitmap, new(0, 0), 0, posX, posY, 15);
        }

        private bool IsColliding(MovableObject image1, MovableObject image2)
        {
            return image1.Collision.CollidesWith(image2.Collision);
        }

        public void Trigger(int amount)
        {
            if (Throwables.Count == 0) return;
            int height = Specsmanager.Height / 4;
            int offset = Specsmanager.Height / 8;
            for (var i = 0; i < amount; i++)
            {
                Objects.Add(new MovableObject(
                    this,
                    item: Throwables.ElementAt(Random.Shared.Next(0, Throwables.Count)),
                   speed: new Vector2(Random.Shared.Next((int)settings.ThrowSetup.ObjectSpeedMin, (int)settings.ThrowSetup.ObjectSpeedMax),
                        -300),
                   rotSpeed: Random.Shared.Next(-20, 20)
                    , x: -100, y: Random.Shared.Next(offset, height), details: 10));
            }
        }

        public void Rain(int amount)
        {
            if (Throwables.Count == 0) return;
            int width = Specsmanager.Width / 4;
            int offset = Specsmanager.Width / 8;
            for (var i = 0; i < amount; i++)
            {
                Objects.Add(new MovableObject(
                    this,
                    details: 10,
                    item: Throwables.ElementAt(Random.Shared.Next(0, Throwables.Count)),
                   speed: new Vector2(Random.Shared.Next(-400, 400),
                   Random.Shared.Next((int)settings.RainSetup.ObjectSpeedMin, (int)settings.RainSetup.ObjectSpeedMax)),
                   rotSpeed: Random.Shared.Next(-20, 20),

                    x: Random.Shared.Next(offset, Specsmanager.Width - offset),
                   y: -Random.Shared.Next(0, 150)));
            }
        }
    }
}
