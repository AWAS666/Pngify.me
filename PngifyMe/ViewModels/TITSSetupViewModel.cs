using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.CoreAudioApi;
using PngifyMe.Layers;
using PngifyMe.Services;
using PngifyMe.Services.Settings;
using PngifyMe.ViewModels.Helper;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PngifyMe.ViewModels;

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
            FileTypeFilter = new[] { FilePickers.AudioAll },
            AllowMultiple = false
        });
        if (!string.IsNullOrEmpty(path.FirstOrDefault()?.Path?.AbsolutePath))
        {
            SettingsManager.Current.Tits.HitSound = WebUtility.UrlDecode(path.FirstOrDefault()?.Path?.AbsolutePath);
            SettingsManager.Current.Tits.HitSoundFileName = Path.GetFileName(SettingsManager.Current.Tits.HitSound);
        }
    }

    public void DefaultSound()
    {
        SettingsManager.Current.Tits.HitSound = string.Empty;
        SettingsManager.Current.Tits.HitSound = string.Empty;
    }

  
}