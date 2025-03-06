using PngifyMe.Layers.Helper;
using PngifyMe.Services.Twitch;
using SkiaSharp;
using System;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;

namespace PngifyMe.Layers.Image
{
    /// <summary>
    /// todo: handle Tiers of subs 
    /// </summary>
    [LayerDescription("TimerLayer")]
    public class TheTimer : ImageLayer
    {
        private SKPaint paint;
        private double positiveTime;
        private double negativeTime;

        [Unit("seconds")]
        public int TotalTime { get; set; } = 3600;

        [Unit("Pixels")]
        public int TextSize { get; set; } = 100;

        [Unit("pixels (center)")]
        public float PosX { get; set; } = 960;

        [Unit("pixels (center)")]
        public float PosY { get; set; } = 540;

        [Unit("seconds per Bit")]
        public int TimeBits { get; set; } = 10;

        [Unit("seconds per Sub")]
        public int TimeSub { get; set; } = 120;

        public override void OnEnter()
        {
            paint = new SKPaint();
            paint.Color = SKColors.Black;
            paint.TextSize = TextSize;
            paint.IsAntialias = true;

            positiveTime = TotalTime;

            TwitchEventSocket.BitsUsed += AddBitTime;
            TwitchEventSocket.Subscription += Sub;
            TwitchEventSocket.SubGift += SubGift;

            // Measure the width of the text
            base.OnEnter();
        }

        private void SubGift(object? sender, ChannelSubscriptionGift e)
        {
            positiveTime += e.Total * TimeSub;
        }

        private void Sub(object? sender, ChannelSubscribe e)
        {
            positiveTime += TimeSub;
        }

        private void AddBitTime(object? sender, ChannelCheer e)
        {
            positiveTime += e.Bits * TimeBits;
        }

        public override void OnCalculateParameters(float dt, ref LayerValues values)
        {
            negativeTime += dt;
        }

        public override void RenderImage(SKCanvas canvas, float offsetX, float offsetY)
        {
            if (negativeTime > positiveTime)
            {
                // timer over, not sure what to do yet
                IsExiting = true;
            }
            string text = TimeSpan.FromSeconds(positiveTime - negativeTime).ToString("hh\\:mm\\:ss\\:ff");
            float width = paint.MeasureText(text);
            canvas.DrawText(text, PosX + offsetX - width / 2, PosY + offsetY, paint);
        }
    }
}
