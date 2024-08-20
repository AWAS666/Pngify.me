using Avalonia.Media.Imaging;
using PngTuberSharp.Services;
using PngTuberSharp.Services.Hotkey;
using PngTuberSharp.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PngTuberSharp.Layers.Microphone
{
    public class MicroPhoneStateLayer
    {
        private Bitmap openImage;
        private Bitmap openBlinkImage;
        private Bitmap closedImage;
        private Bitmap closedBlinkImage;
        private MicroPhoneState current;
        private float transTime;
        private bool blinking;
        private List<Action> callbacks = new();

        public float Interval { get; set; } = 2f;
        public float Time { get; set; } = 0.25f;
        public float CurrentTime { get; private set; }

        public MicroPhoneStateLayer()
        {
            transTime = Interval;
            SwitchState(SettingsManager.Current.Microphone.States.First());
            SetupHotKeys();
        }

        public void Update(float dt, ref LayerValues values)
        {
            CurrentTime += dt;
            if (!blinking && CurrentTime > transTime)
            {
                transTime += (float)SettingsManager.Current.Microphone.BlinkTime;
                blinking = true;
            }
            else if (blinking && CurrentTime > transTime)
            {
                transTime += (float)SettingsManager.Current.Microphone.BlinkInterval;
                blinking = false;
            }

            if (MicrophoneService.Talking)
                values.Image = blinking ? openBlinkImage : openImage;
            else
                values.Image = blinking ? closedBlinkImage : closedImage;
        }

        public void SwitchState(MicroPhoneState state)
        {
            openImage = state.Open.Bitmap;
            openBlinkImage = !string.IsNullOrEmpty(state.OpenBlink.FilePath) ?  state.OpenBlink.Bitmap : state.Open.Bitmap;
            closedImage = state.Closed.Bitmap;
            closedBlinkImage = !string.IsNullOrEmpty(state.ClosedBlink.FilePath) ? state.ClosedBlink.Bitmap : state.Closed.Bitmap;
            current = state;
        }

        public void SetupHotKeys()
        {
            WinHotkey.RemoveCallbacks(callbacks);
            callbacks.Clear();
            foreach (var state in SettingsManager.Current.Microphone.States)
            {
                if (state.Trigger == null)
                    continue;
                var callback = () => SwitchState(state);
                WinHotkey.AddHotkey(state.Trigger.VirtualKeyCode, state.Trigger.Modifiers, callback);
                callbacks.Add(callback);
            }
            // also reloads current just in case
            SwitchState(current);
        }
    }


}
