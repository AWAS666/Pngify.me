using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PngifyMe.Layers;
using PngifyMe.Services;
using PngifyMe.Services.CharacterSetup.Advanced;
using PngifyMe.ViewModels.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;


namespace PngifyMe.ViewModels;

public partial class SpriteSetupViewModel : ObservableObject
{
    public Func<IStorageProvider> GetStorageProvider { get; }
    public SpriteCharacterSettings Settings { get; }

    public SpriteSetupViewModel() : this(null)
    {

    }

    public SpriteSetupViewModel(Func<IStorageProvider> getStorage)
    {
        GetStorageProvider = getStorage;
        Settings = (SpriteCharacterSettings)SettingsManager.Current.Profile.Active.AvatarSettings;
    }

    [RelayCommand]
    public async Task ImportPngtuberPlus()
    {
        var path = await GetStorageProvider().OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Select a Pngtuber Plus save file",
            FileTypeFilter = new[] { FilePickers.PngTuberPlus },
            AllowMultiple = false
        });
        var filePath = path.FirstOrDefault()?.Path?.AbsolutePath;
        if (string.IsNullOrEmpty(filePath)) return;

        var file = await File.ReadAllTextAsync(filePath);
        var obj = JsonSerializer.Deserialize<Dictionary<string, PngTuberPlusObject>>(file);
        var items = obj.Values.ToList();
        var parent = items.First(x => x.parentId == null);
        items.Remove(parent);

        SpriteImage spriteParent = null;
        await Task.Run(async () =>
        {
            // takes some time, maybe show progress
            spriteParent = PngTuberPlusMigrator.MigratePngtuberPlus(parent, items);
        });
        Settings.Parent = spriteParent;
        Settings.SpriteImages = [spriteParent];

        // set states to same amount as parent list, should always be 10 though
        Settings.States = new ObservableCollection<SpriteStates>();
        // init state data
        for (int i = 0; i < spriteParent.LayerStates.Count; i++)
        {
            Settings.States.Add(new()
            {
                Index = i,
                Name = $"Layer {i}"
            });
        }

        LayerManager.CharacterStateHandler.CharacterSetup.RefreshCharacterSettings();
    }
}
