using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PngifyMe.Services.Audio;
public class SoundManager : IDisposable
{
    private readonly WaveOutEvent _outputDevice;
    private readonly MixingSampleProvider _mixer;
    private readonly WaveFormat _mixerFormat;

    public SoundManager(int sampleRate = 44100, int channels = 2, int output = -1)
    {
        // pick your “native” output format here
        _mixerFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
        _mixer = new MixingSampleProvider(_mixerFormat) { ReadFully = true };
        _outputDevice = new WaveOutEvent();
        _outputDevice.DeviceNumber = output;
        _outputDevice.Init(_mixer);
        _outputDevice.Play();
    }

    /// <summary>
    /// Feed me any seekable Stream containing WAV or MP3 data (or extend the sniff logic).
    /// </summary>
    public void PlayStream(Stream stream, float volume = 1.0f)
    {
        var waveStream = CreateWaveStreamFrom(stream);
        PlayStream(waveStream, volume);
    }

    /// <summary>
    /// Deprecated sugar for file paths – now just opens FileStream and calls the Stream overload.
    /// </summary>
    public void PlayFile(string path, float volume = 1.0f)
    {
        var fs = File.OpenRead(path);
        PlayStream(fs, volume);
    }

    private void PlayStream(WaveStream waveStream, float volume)
    {
        ISampleProvider sample = waveStream.ToSampleProvider();

        if (!waveStream.WaveFormat.Equals(_mixerFormat))
            sample = new WdlResamplingSampleProvider(sample, _mixerFormat.SampleRate);

        sample = new VolumeSampleProvider(sample) { Volume = volume };

        var wrapper = new AutoRemoveSampleProvider(sample, waveStream, _mixer);

        lock (_mixer)
        {
            _mixer.AddMixerInput(wrapper);
        }
    }

    private WaveStream CreateWaveStreamFrom(Stream input)
    {
        // need random‑access to sniff header; if not seekable, copy to memory
        if (!input.CanSeek)
        {
            var mem = new MemoryStream();
            input.CopyTo(mem);
            mem.Position = 0;
            input = mem;
        }

        // read first 4 bytes
        input.Position = 0;
        var br = new BinaryReader(input, System.Text.Encoding.ASCII, leaveOpen: true);
        uint sig = br.ReadUInt32();
        input.Position = 0;

        // “RIFF” little‑endian → WAV
        if (sig == 0x46464952)
            return new WaveFileReader(input);

        // check for “ID3” tag or MP3 frame sync (0xFFE____)
        var id3 = System.Text.Encoding.ASCII.GetString(br.ReadBytes(3));
        input.Position = 0;
        if (id3 == "ID3" || (sig & 0xFFE00000) == 0xFFE00000)
            return new Mp3FileReader(input);

        // else… try MP3 anyway
        try
        {
            return new Mp3FileReader(input);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Unsupported audio format or corrupted stream", ex);
        }
    }

    public void Dispose()
    {
        _outputDevice?.Stop();
        _outputDevice?.Dispose();
        // AutoRemoveSampleProvider will dispose each WaveStream when it ends
    }

    private class AutoRemoveSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider _source;
        private readonly WaveStream _toDispose;
        private readonly MixingSampleProvider _mixer;
        private bool _removed;

        public AutoRemoveSampleProvider(ISampleProvider source, WaveStream toDispose, MixingSampleProvider mixer)
        {

            _source = source;
            _toDispose = toDispose;
            _mixer = mixer;
        }

        public WaveFormat WaveFormat => _source.WaveFormat;

        public int Read(float[] buffer, int offset, int count)
        {
            int read = _source.Read(buffer, offset, count);
            if (read == 0 && !_removed)
            {
                lock (_mixer)
                {
                    _mixer.RemoveMixerInput(this);
                }
                _toDispose.Dispose();
                _removed = true;
            }
            return read;
        }
    }
}
