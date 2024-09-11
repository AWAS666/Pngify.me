using CppSharp.Types.Std;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using PngifyMe.Services.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PngifyMe.Services
{
    public static class AudioService
    {
        private static WaveInEvent _waveIn;
        private static float last = 0f;
        private static ProfileType current;

        public static MicroPhoneSettings Settings => SettingsManager.Current.Profile.Active.MicroPhone;
        public static bool Talking { get; private set; }

        public static event EventHandler<MicroPhoneLevel> LevelChanged;

        static AudioService()
        {
            ChangeMode(SettingsManager.Current.Profile.Active.Type);
        }

        public static void ChangeMode(ProfileType type)
        {
            switch (type)
            {
                case ProfileType.Human:
                    Start();
                    break;
                case ProfileType.TTS:
                    Stop();
                    break;
                default:
                    break;
            }
            current = type;
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
            float current = Current(max);
            Talking = current > Settings.ThreshHold;
            LevelChanged?.Invoke(null, new MicroPhoneLevel(Talking, (int)current));
        }

        private static float Current(float max)
        {
            float current = (max * 4 + last * Settings.Smoothing) / (Settings.Smoothing + 4);
            last = current;
            return current * 100;
        }

        public static void Start()
        {
            _waveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(8000, 1),
                DeviceNumber = Settings.DeviceIn
            };

            _waveIn.DataAvailable += OnDataAvailable;
            _waveIn.StartRecording();
        }

        public static async Task PlaySound(Stream stream)
        {
            WaveStream reader = new Mp3FileReader(stream);
            using var player = new WaveOutEvent();
            player.DeviceNumber = Settings.DeviceOut;
            var meter = new MeteringSampleProvider(reader.ToSampleProvider());
            meter.StreamVolume += (s, e) =>
                        {
                            float value = e.MaxSampleValues.OrderDescending().First();
                            float current = Current(value);
                            Talking = current > Settings.ThreshHold;
                            LevelChanged?.Invoke(null, new MicroPhoneLevel(Talking, (int)current));
                        };

            player.Init(meter);
            player.Play();

            while (player.PlaybackState == PlaybackState.Playing)
            {
                await Task.Delay(500);
            }
            // cleanup
            stream.Dispose();
            Talking = false;
            // close at the end
            LevelChanged?.Invoke(null, new MicroPhoneLevel(false, 0));
        }

        public static void Stop()
        {
            _waveIn?.StopRecording();
        }

        public static List<string> GetAllInDevices()
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

        public static List<string> GetAllOutDevices()
        {
            var list = new List<string>();
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

            foreach (var device in devices)
            {
                list.Add(device.FriendlyName);
            }
            return list;
        }

        public static async Task Restart()
        {
            Stop();
            ChangeMode(current);
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
