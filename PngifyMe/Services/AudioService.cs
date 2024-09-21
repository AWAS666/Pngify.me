using NAudio.Wave;
using PngifyMe.Services.Settings;
using PortAudioSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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
    public static MicroPhoneSettings Settings => SettingsManager.Current.Profile.Active.MicroPhone;
    private static float last = 0f;
    private static ProfileType current;
    private static Stream stream;

    public static bool Talking { get; private set; }
    public static List<AudioDeviceConfig> InputDevices { get; }
    public static List<AudioDeviceConfig> OutputDevices { get; }

    public static event EventHandler<MicroPhoneLevel> LevelChanged;

    static AudioService()
    {
        PortAudio.Initialize();
        Log.Debug($"{PortAudio.VersionInfo.versionText}");
        InputDevices = GetAllInDevices();
        OutputDevices = GetAllOutDevices();
        ChangeMode(SettingsManager.Current.Profile.Active.Type);
    }

    public static void Init()
    {
        // init stuff I guess
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
        int host = 0;
        var list = new List<AudioDeviceConfig>();

        for (int i = 0; i != PortAudio.DeviceCount; ++i)
        {
            DeviceInfo deviceInfo = PortAudio.GetDeviceInfo(i);
            if (deviceInfo.maxInputChannels > 0 && deviceInfo.hostApi == host)
            {
                list.Add(new AudioDeviceConfig()
                {
                    Id = i,
                    Name = deviceInfo.name,
                    Channels = deviceInfo.maxInputChannels,
                    Host = deviceInfo.hostApi,
                    Struct = deviceInfo.structVersion,
                });
                host = deviceInfo.hostApi;
            }
        }

        return list;
    }

    private static List<AudioDeviceConfig> GetAllOutDevices()
    {
        var list = new List<AudioDeviceConfig>();

        for (int i = 0; i != PortAudio.DeviceCount; ++i)
        {
            DeviceInfo deviceInfo = PortAudio.GetDeviceInfo(i);
            if (deviceInfo.maxOutputChannels > 0)
                list.Add(new AudioDeviceConfig()
                {
                    Id = i,
                    Name = deviceInfo.name,
                    Channels = deviceInfo.maxOutputChannels,
                    Host = deviceInfo.hostApi,
                    Struct = deviceInfo.structVersion,
                });
        }

        return list;
    }


    public static async Task PlaySound(System.IO.Stream wavstream)
    {
        int deviceIndex = Settings.DeviceOut;

        DeviceInfo info = PortAudio.GetDeviceInfo(deviceIndex);

        using var waveFileReader = new Mp3FileReader(wavstream);
        int sampleRate = waveFileReader.WaveFormat.SampleRate;
        int channelCount = waveFileReader.WaveFormat.Channels;
        var waveFormat = waveFileReader.WaveFormat;

        StreamParameters param = new StreamParameters
        {
            device = deviceIndex,
            channelCount = channelCount,
            sampleFormat = SampleFormat.Float32,
            suggestedLatency = info.defaultLowOutputLatency,
            hostApiSpecificStreamInfo = IntPtr.Zero
        };

        Stream.Callback playCallback = (IntPtr input, IntPtr output, uint frameCount,
            ref StreamCallbackTimeInfo timeInfo, StreamCallbackFlags statusFlags, IntPtr userData) =>
        {
            float[] buffer = new float[frameCount * channelCount];
            byte[] waveBuffer = new byte[frameCount * waveFormat.BlockAlign]; // To store raw PCM bytes

            int bytesRead = waveFileReader.Read(waveBuffer, 0, waveBuffer.Length);
            if (bytesRead == 0)
            {
                return StreamCallbackResult.Complete; // End of stream
            }

            // Convert PCM bytes to float
            if (waveFormat.BitsPerSample == 16)
            {
                // 16-bit PCM -> float [-1.0, 1.0]
                int samplesRead = bytesRead / 2; // 2 bytes per sample for 16-bit PCM
                for (int i = 0; i < samplesRead; i++)
                {
                    short sampleValue = BitConverter.ToInt16(waveBuffer, i * 2);
                    buffer[i] = sampleValue / 32768f; // Convert 16-bit PCM to float
                }
            }
            else if (waveFormat.BitsPerSample == 32)
            {
                // 32-bit PCM -> float directly
                int samplesRead = bytesRead / 4; // 4 bytes per sample for 32-bit float PCM
                for (int i = 0; i < samplesRead; i++)
                {
                    buffer[i] = BitConverter.ToSingle(waveBuffer, i * 4); // Read as float directly
                }
            }

            float value = buffer.Max();
            float current = Current(value);
            Talking = current > Settings.ThreshHold;
            LevelChanged?.Invoke(null, new MicroPhoneLevel(Talking, (int)current));

            Marshal.Copy(buffer, 0, output, buffer.Length);
            return StreamCallbackResult.Continue;
        };

        using var stream = new Stream(
            inParams: null,
            outParams: param,
            sampleRate: sampleRate,
            framesPerBuffer: 0,
            streamFlags: StreamFlags.ClipOff,
            callback: playCallback,
            userData: IntPtr.Zero
        );

        stream.Start();

        while (stream.IsActive)
            await Task.Delay(50);

        stream.Stop();
        while (!stream.IsStopped)
            await Task.Delay(50);
    }

    public static async Task PlaySoundWav(System.IO.Stream wavstream, float volume, bool ignoreSpeech = false)
    {
        try
        {
            int deviceIndex = Settings.DeviceOut;

            DeviceInfo info = PortAudio.GetDeviceInfo(deviceIndex);

            using var waveFileReader = new WaveFileReader(wavstream);
            int sampleRate = waveFileReader.WaveFormat.SampleRate;
            int channelCount = waveFileReader.WaveFormat.Channels;
            var waveFormat = waveFileReader.WaveFormat;

            StreamParameters param = new StreamParameters
            {
                device = deviceIndex,
                channelCount = channelCount,
                sampleFormat = SampleFormat.Float32,
                suggestedLatency = info.defaultLowOutputLatency,
                hostApiSpecificStreamInfo = IntPtr.Zero
            };

            Stream.Callback playCallback = (IntPtr input, IntPtr output, uint frameCount,
                ref StreamCallbackTimeInfo timeInfo, StreamCallbackFlags statusFlags, IntPtr userData) =>
            {
                // wav variant
                float[] buffer = new float[frameCount * channelCount];
                for (int i = 0; i < frameCount; i++)
                {
                    var sampleFrame = waveFileReader.ReadNextSampleFrame();
                    if (sampleFrame == null)
                    {
                        return StreamCallbackResult.Complete;
                    }

                    for (int channel = 0; channel < channelCount; channel++)
                    {
                        buffer[i * channelCount + channel] = sampleFrame[channel] * volume;
                    }
                }

                if (!ignoreSpeech)
                {
                    float value = buffer.Max();
                    float current = Current(value);
                    Talking = current > Settings.ThreshHold;
                    LevelChanged?.Invoke(null, new MicroPhoneLevel(Talking, (int)current));
                }

                Marshal.Copy(buffer, 0, output, buffer.Length);
                return StreamCallbackResult.Continue;
            };

            using var stream = new Stream(
                inParams: null,
                outParams: param,
                sampleRate: sampleRate,
                framesPerBuffer: 0,
                streamFlags: StreamFlags.ClipOff,
                callback: playCallback,
                userData: IntPtr.Zero
            );

            stream.Start();

            while (stream.IsActive)
                await Task.Delay(50);

            stream.Stop();
            while (!stream.IsStopped)
                await Task.Delay(50);

        }
        catch (Exception e)
        {
            Log.Error("Error in audio: " + e.Message, e);
        }
    }

    public static void Start()
    {
        int deviceIndex = Settings.DeviceIn;

        DeviceInfo info = PortAudio.GetDeviceInfo(deviceIndex);

        StreamParameters param = new StreamParameters();
        param.device = deviceIndex;
        param.channelCount = info.maxInputChannels;
        param.sampleFormat = SampleFormat.Float32;
        param.suggestedLatency = info.defaultLowInputLatency;
        param.hostApiSpecificStreamInfo = IntPtr.Zero;

        static StreamCallbackResult callback(IntPtr input, IntPtr output,
        UInt32 frameCount,
        ref StreamCallbackTimeInfo timeInfo,
        StreamCallbackFlags statusFlags,
        IntPtr userData
        )
        {
            float[] samples = new float[frameCount];
            Marshal.Copy(input, samples, 0, (Int32)frameCount);

            Debug.WriteLine(samples.Max());

            float current = Current(samples.Max() * 2);
            Talking = current > Settings.ThreshHold;
            LevelChanged?.Invoke(null, new MicroPhoneLevel(Talking, (int)current));

            return StreamCallbackResult.Continue;
        }

        stream = new(inParams: param, outParams: null, sampleRate: info.defaultSampleRate,
           framesPerBuffer: 0,
           streamFlags: StreamFlags.ClipOff,
           callback: callback,
           userData: IntPtr.Zero
           );

        stream.Start();
    }

    public static void Stop()
    {
        if (stream != null)
        {
            stream.Stop();
        }
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