using Avalonia.Media.Imaging;
using PngTuberSharp.Services;
using SkiaSharp;
using System;

namespace PngTuberSharp.Layers.Microphone
{
    public class BasicMicroPhoneLayer : MicroPhoneLayer
    {
        private SKBitmap openImage;
        private SKBitmap closedImage;      

        private void RefreshImage(object? sender = null, EventArgs e = null)
        {
            openImage = SKBitmap.Decode(SettingsManager.Current.Avatar.Open);
            closedImage = SKBitmap.Decode(SettingsManager.Current.Avatar.Closed);
        }

        public override void Update(float dt, ref LayerValues values)
        {
            if (MicrophoneService.Talking)
                values.Image = openImage;
            else
                values.Image = closedImage;
        }

        public override void Enter()
        {
            SettingsManager.Current.Avatar.Refresh += RefreshImage;
            RefreshImage();
        }        
    }
}
