using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Services.Settings;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
    private double blinkInterval = 0.25f;
    public ObservableCollection<SpriteStates> States { get; set; } = new();
    public List<string> AvailableStates() => States.Select(s => s.Name).ToList();

    public SpriteCharacterSettings()
    {
        spriteImages = [parent];
    }
}


public class SpriteStates
{
    public string Name { get; set; }
    public int Index { get; set; }
    public HotkeyTrigger? Trigger { get; set; } = null;
}
