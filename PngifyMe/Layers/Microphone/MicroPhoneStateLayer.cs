using PngifyMe.Services;
using PngifyMe.Services.Hotkey;
using PngifyMe.Services.Settings;
using PngifyMe.Services.Settings.Images;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PngifyMe.Layers.Microphone
{
    public class MicroPhoneStateLayer
    {
        private BaseImage openImage;
        private BaseImage openBlinkImage;
        private BaseImage closedImage;
        private BaseImage closedBlinkImage;
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
            var def = SettingsManager.Current.Microphone.States.FirstOrDefault(x => x.Default);
            if (def == null)
            {
                def = SettingsManager.Current.Microphone.States.First();
                def.Default = true;
            }
            SwitchState(def);
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

            BaseImage baseImage = null;
            if (MicrophoneService.Talking)
                baseImage = blinking ? openBlinkImage : openImage;
            else
                baseImage = blinking ? closedBlinkImage : closedImage;
            values.Image = baseImage.GetImage(TimeSpan.FromSeconds(CurrentTime));
        }

        public void SwitchState(MicroPhoneState state, bool reload = false)
        {
            if (current == state && !reload) return;

            openImage = state.Open.Bitmap;
            openBlinkImage = !string.IsNullOrEmpty(state.OpenBlink.FilePath) ? state.OpenBlink.Bitmap : state.Open.Bitmap;
            closedImage = state.Closed.Bitmap;
            closedBlinkImage = !string.IsNullOrEmpty(state.ClosedBlink.FilePath) ? state.ClosedBlink.Bitmap : state.Closed.Bitmap;
            current = state;
        }

        public void ToggleState(MicroPhoneState state)
        {
            if (current == state)
            {
                SwitchState(SettingsManager.Current.Microphone.States.First(x => x.Default));
            }
            else
            {
                SwitchState(state);
            }
        }

        public void SetupHotKeys()
        {
            WinHotkey.RemoveCallbacks(callbacks);
            callbacks.Clear();
            foreach (var state in SettingsManager.Current.Microphone.States)
            {
                if (state.Trigger == null)
                    continue;
                var callback = () => ToggleState(state);
                WinHotkey.AddHotkey(state.Trigger.VirtualKeyCode, state.Trigger.Modifiers, callback);
                callbacks.Add(callback);
            }
            // also reloads current just in case
            SwitchState(current, reload: true);
        }
    }


}
