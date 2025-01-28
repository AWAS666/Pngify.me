using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Services.CharacterSetup.Basic;
using PngifyMe.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngifyMe.Services.CharacterSetup.Advanced;
public partial class SpriteCharacterSettings : ObservableObject, IAvatarSettings
{
    public SpriteImage Parent { get; set; } = new();
    public double BlinkTime { get; set; } = 0.25f;
    public double BlinkInterval { get; set; } = 3f;
    public int ActiveLayer { get; set; } = 0;
    public List<SpriteStates> States { get; set; } = new();

    public List<string> AvailableStates() => States.Select(s => s.Name).ToList();
}


public class SpriteStates
{
    public string Name { get; set; }
    public int Index { get; set; }
    public HotkeyTrigger? Trigger { get; set; } = null;
}
