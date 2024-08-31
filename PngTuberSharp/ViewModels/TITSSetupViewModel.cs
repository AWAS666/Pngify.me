using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.CoreAudioApi;
using PngTuberSharp.Layers;
using PngTuberSharp.Services;
using PngTuberSharp.Services.Settings;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PngTuberSharp.ViewModels;

public partial class TITSSetupViewModel : ObservableObject
{
    private Func<IStorageProvider> GetStorageProvider;

    public TitsSettings Settings { get; }

    public TITSSetupViewModel() : this(null)
    {

    }

    public TITSSetupViewModel(Func<IStorageProvider> storageProvider)
    {
        GetStorageProvider = storageProvider;
        Settings = SettingsManager.Current.Tits;
    }

    public void Trigger()
    {
        LayerManager.ThrowingSystem.Trigger(Random.Shared.Next(5, 10));
    }

    public async Task ChangeSound()
    {
        var path = await GetStorageProvider().OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Select a wav file",
            FileTypeFilter = new[] { AudioALl },
            AllowMultiple = false
        });
        if (!string.IsNullOrEmpty(path.FirstOrDefault()?.Path?.AbsolutePath))
            SettingsManager.Current.Tits.HitSound = WebUtility.UrlDecode(path.FirstOrDefault()?.Path?.AbsolutePath);
    }

    public void DefaultSound()
    {
        SettingsManager.Current.Tits.HitSound = string.Empty;
    }

    public static FilePickerFileType AudioALl { get; } = new("Wav")
    {
        Patterns = new[] { "*.wav", },
        AppleUniformTypeIdentifiers = new[] { "public.image" },
    };
}