using NAudio.Wave;
using PngTuberSharp.Services.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PngTuberSharp.Services
{
    public static class MicrophoneService
    {
        private static WaveInEvent _waveIn;
        public static MicroPhoneSettings Settings { get; private set; } = SettingsManager.Current.Microphone;
        public static bool Talking { get; private set; }

        public static event EventHandler<MicroPhoneLevel> LevelChanged;

        static MicrophoneService()
        {
            Start();
        }

        private static void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            float max = 0;
            // interpret as 16 bit audio
            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                short sample = (short)((e.Buffer[index + 1] << 8) |
                                        e.Buffer[index + 0]);
                // to floating point
                var sample32 = sample / 32768f;
                // absolute value 
                if (sample32 < 0) sample32 = -sample32;
                // is this the max value?
                if (sample32 > max) max = sample32;
            }
            Talking = max * 100f > Settings.ThreshHold;
            LevelChanged?.Invoke(null, new MicroPhoneLevel(Talking, (int)(max * 100f)));
            //Debug.WriteLine(max * 100f);
        }

        public static void Start()
        {
            _waveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(8000, 1),
                DeviceNumber = Settings.Device
            };

            _waveIn.DataAvailable += OnDataAvailable;
            _waveIn.StartRecording();
        }

        public static void Stop()
        {
            _waveIn.StopRecording();
        }

        public static List<string> GetAllDevices()
        {
            var list = new List<string>();
            int waveInDevices = WaveInEvent.DeviceCount;

            // Loop through all input devices and display their names
            for (int i = 0; i < waveInDevices; i++)
            {
                var deviceInfo = WaveInEvent.GetCapabilities(i);
                list.Add(deviceInfo.ProductName);
            }
            return list;
        }

        public static async Task Restart()
        {
            Stop();
            Start();
        }
    }

    public struct MicroPhoneLevel
    {
        public bool Threshold { get; set; }
        public int Percent { get; set; }
        public MicroPhoneLevel(bool treshhold, int percent)
        {
            Threshold = treshhold;
            Percent = percent;
        }
    }
}
