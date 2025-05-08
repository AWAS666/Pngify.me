using Avalonia.Controls;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using PngifyMe.Services.Audio;
using PngifyMe.Services.Settings;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
    private static WaveInEvent _waveIn;

    public static bool Talking { get; private set; }
    public static List<AudioDeviceConfig> InputDevices { get; }
    public static List<AudioDeviceConfig> OutputDevices { get; }

    public static event EventHandler<MicroPhoneLevel> LevelChanged;
    public static SoundManager SoundManager { get; private set; } = new SoundManager(output:Settings.DeviceOut);

    static AudioService()
    {
        InputDevices = GetAllInDevices();
        OutputDevices = GetAllOutDevices();
        if (WaveInEvent.DeviceCount == 0)
            Log.Error("No Audio input devices found...");
        if (Settings.DeviceIn >= WaveInEvent.DeviceCount)
            Settings.DeviceIn = 0;

        using var mmEnum = new MMDeviceEnumerator();
        var devices = mmEnum.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
        if (devices.Count == 0)
            Log.Error("No Audio ouput devices found...");
        if (Settings.DeviceOut >= devices.Count)
            Settings.DeviceOut = 0;

    }

    public static void Init()
    {
        ChangeMode(SettingsManager.Current.Profile.Active.Type);
        Settings.DeviceOutChanged += OutputChanged;
    }

    private static void OutputChanged(object? sender, int e)
    {
        SoundManager.Dispose();
        SoundManager = new SoundManager(output: e);
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
        int waveInDevices = WaveInEvent.DeviceCount;

        // Loop through all input devices and display their names
        for (int i = 0; i < waveInDevices; i++)
        {
            var deviceInfo = WaveInEvent.GetCapabilities(i);
            list.Add(new AudioDeviceConfig()
            {
                Id = i,
                Name = deviceInfo.ProductName,
                Channels = deviceInfo.Channels,
            });
        }
        return list;
    }

    private static List<AudioDeviceConfig> GetAllOutDevices()
    {
        var list = new List<AudioDeviceConfig>();
        var enumerator = new MMDeviceEnumerator();
        var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

        int i = 0;
        foreach (var deviceInfo in devices)
        {
            list.Add(new AudioDeviceConfig()
            {
                Id = i++,
                Name = deviceInfo.FriendlyName,
            });
        }
        return list;
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


    public static async Task PlaySoundWav(Stream wavstream, float volume)
    {
        try
        {
            SoundManager.PlayStream(wavstream, volume);
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
            int deviceIndex = Settings.DeviceIn;

            _waveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(8000, 1),
                DeviceNumber = Settings.DeviceIn
            };

            _waveIn.DataAvailable += OnDataAvailable;
            _waveIn.StartRecording();
        }
        catch (Exception e)
        {
            Log.Fatal(e, $"Fatal error in audio service, couldn't start recording: {e.Message}");
        }
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

    public static void Stop()
    {
        if (_waveIn == null) return;
        _waveIn.StopRecording();
        _waveIn.DataAvailable -= OnDataAvailable;
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
    public int Channels { get; internal set; }
    public int Struct { get; internal set; }
    public int Host { get; internal set; }
}