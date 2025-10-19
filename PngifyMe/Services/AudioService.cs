using Avalonia.Controls;
using PngifyMe.Services.Settings;
using Serilog;
using SoundFlow.Abstracts.Devices;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Enums;
using SoundFlow.Providers;
using SoundFlow.Structs;
using SoundFlow.Visualization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ursa.Controls;

namespace PngifyMe.Services;

/// <summary>
/// Implementing:
/// https://github.com/csukuangfj/PortAudioSharp2
/// possible way to completely migrate to naudio.core:
/// https://github.com/naudio/NLayer
/// </summary>
public static class AudioService
{
    public static MicSettings Settings => SettingsManager.Current.Profile.Active.MicSettings;
    private static float last = 0f;
    private static ProfileType current;

    public static bool Talking { get; private set; }

    private static MiniAudioEngine engine;
    private static AudioCaptureDevice captureDevice;
    private static AudioPlaybackDevice playbackDevice;

    public static List<AudioDeviceConfig> InputDevices { get; }
    public static List<AudioDeviceConfig> OutputDevices { get; }

    public static event EventHandler<MicroPhoneLevel> LevelChanged;
    //public static SoundManager SoundManager { get; private set; } = new SoundManager(output: Settings.DeviceOut);

    static AudioService()
    {
        try
        {
            engine = new MiniAudioEngine();
            engine.UpdateDevicesInfo();

            InputDevices = GetAllInDevices();
            OutputDevices = GetAllOutDevices();

            var count = engine.CaptureDevices.Length;
            if (count == 0)
                Log.Error("No Audio input devices found...");

            count = engine.PlaybackDevices.Length;
            if (count == 0)
                Log.Error("No Audio ouput devices found...");
        }
        catch (Exception e)
        {

            Log.Fatal(e, $"Error in AudioService init: {e.Message}");
        }
    }

    public static void Init()
    {
        ChangeMode(SettingsManager.Current.Profile.Active.Type);
        Settings.DeviceOutChanged += OutputChanged;

        var format = new AudioFormat
        {
            SampleRate = 48000,
            Channels = 2,
            Format = SampleFormat.F32
        };

        var outDevice = engine.PlaybackDevices.FirstOrDefault(x => string.Compare(x.Name, Settings.DeviceOutName, true) == 0);
        if (outDevice == default)
        {
            outDevice = engine.PlaybackDevices.ElementAtOrDefault(Settings.DeviceOut);
        }
        playbackDevice = engine.InitializePlaybackDevice(outDevice, format);
        playbackDevice.Start();
    }

    private static void OutputChanged(object? sender, int e)
    {
        var newDevice = engine.PlaybackDevices.ElementAtOrDefault(e);
        playbackDevice = engine.SwitchDevice(playbackDevice, newDevice);
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

    private static List<AudioDeviceConfig> GetAllInDevices()
    {
        var list = new List<AudioDeviceConfig>();
        int i = 0;
        foreach (var item in engine.CaptureDevices)
        {
            list.Add(new AudioDeviceConfig()
            {
                Id = i++,
                Name = item.Name,
            });
        }
        return list;
    }

    private static List<AudioDeviceConfig> GetAllOutDevices()
    {
        var list = new List<AudioDeviceConfig>();
        int i = 0;
        foreach (var item in engine.PlaybackDevices)
        {
            list.Add(new AudioDeviceConfig()
            {
                Id = i++,
                Name = item.Name,
            });
        }
        return list;
    }

    public static async Task PlaySound(Stream stream)
    {
        var format = new AudioFormat
        {
            SampleRate = 48000,
            Channels = 2,
            Format = SampleFormat.F32
        };
        using var dataProvider = new StreamDataProvider(engine, format, stream);
        var player = new SoundPlayer(engine, format, dataProvider)
        {
        };

        var levelMeter = new LevelMeterAnalyzer(format);
        player.AddAnalyzer(levelMeter);

        playbackDevice.MasterMixer.AddComponent(player);

        player.Play();

        while (player.State == PlaybackState.Playing)
        {
            float value = levelMeter.Peak;
            float current = Current(value);
            Talking = current > Settings.ThreshHold;
            LevelChanged?.Invoke(null, new MicroPhoneLevel(Talking, (int)current));
            await Task.Delay(10);
        }   
        player.Dispose();
        // close at the end
        Talking = false;
        LevelChanged?.Invoke(null, new MicroPhoneLevel(false, 0));
    }


    public static async Task PlaySoundWav(Stream wavstream, float volume)
    {
        try
        {
            var format = new AudioFormat
            {
                SampleRate = 48000,
                Channels = 2,
                Format = SampleFormat.F32
            };

            using var dataProvider = new StreamDataProvider(engine, format, wavstream);
            var player = new SoundPlayer(engine, format, dataProvider)
            {
                Volume = volume
            };

            playbackDevice.MasterMixer.AddComponent(player);
            player.Play();
            while (player.State == PlaybackState.Playing)
            {
                await Task.Delay(10);
            }
            player.Dispose();
        }
        catch (Exception e)
        {
            Log.Error("Error in audio: " + e.Message, e);
        }
    }

    public static void Start()
    {
        try
        {
            var device = engine.CaptureDevices.FirstOrDefault(x => string.Compare(x.Name, Settings.DeviceInName, true) == 0);
            // if no string match, try index match
            if (device == default)
            {
                device = engine.CaptureDevices.ElementAtOrDefault(Settings.DeviceIn);
            }

            captureDevice = engine.InitializeCaptureDevice(device, AudioFormat.Telephony);

            captureDevice.OnAudioProcessed += OnDataAvailable;
            captureDevice.Start();
        }
        catch (Exception e)
        {
            Log.Fatal(e, $"Fatal error in audio service, couldn't start recording: {e.Message}");
        }
    }

    private static void OnDataAvailable(Span<float> samples, Capability capability)
    {
        float max = 0;
        foreach (var sample in samples)
        {
            var val = Math.Abs(sample);
            if (val > max)
                max = val;
        }
        float current = Current(max);
        Talking = current > Settings.ThreshHold;
        LevelChanged?.Invoke(null, new MicroPhoneLevel(Talking, (int)current));
    }

    public static void Stop()
    {
        if (captureDevice == null) return;
        captureDevice.Stop();
        captureDevice.OnAudioProcessed -= OnDataAvailable;
        captureDevice.Dispose();
    }

    public static void Restart()
    {
        Stop();
        ChangeMode(current);
    }

    private static float Current(float max)
    {
        float current = (max * 4 + last * Settings.Smoothing) / (Settings.Smoothing + 4);
        last = current;
        return current * 100;
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

public class AudioDeviceConfig
{
    public int Id { get; set; }
    public string Name { get; set; }
}