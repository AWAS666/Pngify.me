using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Services.Settings;
using PngifyMe.ViewModels.Helper;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;

namespace PngifyMe.Services.CharacterSetup.Advanced;
public partial class SpriteCharacterSettings : ObservableObject, IAvatarSettings
{
    [ObservableProperty]
    private List<SpriteImage> spriteImages;

    [ObservableProperty]
    private SpriteImage parent = new();

    [ObservableProperty]
    private SpriteImage selected = new();

    [ObservableProperty]
    private double blinkTime = 0.25f;

    [ObservableProperty]
    private double blinkInterval = 3f;

    [ObservableProperty]
    private SpriteStates activateState;
    public ObservableCollection<SpriteStates> States { get; set; } = new();
    public List<string> AvailableStates() => States.Select(s => s.Name).ToList();

    public SpriteCharacterSettings()
    {
        spriteImages = [parent];
    }
}


public partial class SpriteStates : ObservableObject
{
    public string Name { get; set; }
    public int Index { get; set; }

    [ObservableProperty]
    private Trigger trigger = new HotkeyTrigger();

}
