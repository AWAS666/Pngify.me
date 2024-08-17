﻿using Avalonia.Media.Imaging;
using PngTuberSharp.Services;
using System;

namespace PngTuberSharp.Layers.Microphone
{
    public class BasicMicroPhoneLayer : PermaLayer
    {
        private Bitmap openImage;
        private Bitmap closedImage;

        public BasicMicroPhoneLayer()
        {
            Unique = true;
        }

        private void RefreshImage(object? sender = null, EventArgs e = null)
        {
            openImage = new Bitmap(SettingsManager.Current.Avatar.Open);
            closedImage = new Bitmap(SettingsManager.Current.Avatar.Closed);
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
            SettingsManager.Current.Avatar.Refresh += RefreshImage;
            RefreshImage();
        }        
    }
}
