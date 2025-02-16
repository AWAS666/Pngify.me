using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PngifyMe.Layers;
using PngifyMe.Services;
using PngifyMe.Services.CharacterSetup.Advanced;
using PngifyMe.ViewModels.Helper;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
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
        filePath = WebUtility.UrlDecode(filePath);

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
        Settings.States.Clear();
        // init state data
        for (int i = 0; i < spriteParent.LayerStates.Count; i++)
        {
            Settings.States.Add(new()
            {
                Index = i,
                Name = $"Layer {i}"
            });
        }
        Settings.SetupTriggers();

        LayerManager.CharacterStateHandler.CharacterSetup.RefreshCharacterSettings();
    }

    [RelayCommand]
    public async Task ImportFromPngs()
    {
        var path = await GetStorageProvider().OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Select images",
            FileTypeFilter = new[] { FilePickers.ImageAll },
            AllowMultiple = true
        });
        var parent = new SpriteImage();
        parent.LayerStates = SpriteImage.InitLayerStates(Settings.States.Count);
        Settings.Parent = parent;
        Settings.SpriteImages = [parent];
        if (Settings.States.Count == 0)
        {
            Settings.States.Add(new()
            {
                Name = "Layer 1",
                Index = 0
            });
        }

        string filePath = string.Empty;
        foreach (var item in path)
        {
            try
            {
                filePath = WebUtility.UrlDecode(item.Path.AbsolutePath);
                var sprite = new SpriteImage();
                await Task.Run(async () =>
                {
                    // takes some time, maybe show progress
                    PngTuberPlusMigrator.LoadFromFile(filePath, sprite);
                });
                sprite.LayerStates = SpriteImage.InitLayerStates(Settings.States.Count);
                parent.Children.Add(sprite);

            }
            catch (Exception e)
            {
                Log.Error($"{Path.GetFileNameWithoutExtension(filePath)}{e.Message}");
            }
        }

        Settings.SetupTriggers();
        LayerManager.CharacterStateHandler.CharacterSetup.RefreshCharacterSettings();
    }


    [RelayCommand]
    public void AddSprite()
    {
        Settings.Parent.AddSprite();
    }


    [RelayCommand]
    public void SetupTriggers()
    {
        Settings.SetupTriggers();
    }

    [RelayCommand]
    public void AddTrigger()
    {
        Settings.States.Add(new()
        {
            Index = Settings.States.Count,
        });
        Settings.Parent.AddNewState(Settings.States.Count);
    }

    [RelayCommand]
    public void RemoveTrigger(SpriteStates state)
    {
        Settings.States.Remove(state);
    }

    [RelayCommand]
    public void ClearSelect()
    {
        Settings.Selected = null;
    }
}
