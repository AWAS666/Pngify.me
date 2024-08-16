using Avalonia.Media.Imaging;
using PngTuberSharp.Layers.Movements;
using PngTuberSharp.Services;
using System;

namespace PngTuberSharp.Layers.Microphone
{
    public class BasicMicroPhoneLayer : BaseLayer
    {
        private Bitmap openImage;
        private Bitmap closedImage;

        public BasicMicroPhoneLayer()
        {
            Unique = true;
            SettingsManager.Current.Avatar.Refresh += RefreshImage;
        }

        private void RefreshImage(object? sender, EventArgs e)
        {
            OnEnter();
        }

        public override void OnCalculateParameters(float dt, ref LayerValues values)
        {
            if (MicrophoneService.Talking)
                values.Image = openImage;
            else
                values.Image = closedImage;
        }

        public override void OnEnter()
        {
            openImage = new Bitmap(SettingsManager.Current.Avatar.Open);
            closedImage = new Bitmap(SettingsManager.Current.Avatar.Closed);
        }

        public override void OnExit()
        {
        }

        public override void OnUpdate(float dt, float time)
        {
        }

        public override void OnUpdateEnter(float dt, float fraction)
        {
        }

        public override void OnUpdateExit(float dt, float fraction)
        {
        }
    }
}
