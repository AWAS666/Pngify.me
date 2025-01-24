using PngifyMe.Services.CharacterSetup.Images;
using PngifyMe.Services.Settings;

namespace PngifyMe.Services.CharacterSetup.Basic;

public class CharacterState
{
    public string Name { get; set; }
    public bool Default { get; set; }
    public ImageSetting Open { get; set; } = new();
    public ImageSetting OpenBlink { get; set; } = new();
    public ImageSetting Closed { get; set; } = new();
    public ImageSetting ClosedBlink { get; set; } = new();
    public ImageSetting EntryImage { get; set; } = new();
    public ImageSetting ExitImage { get; set; } = new();

    public HotkeyTrigger? Trigger { get; set; } = null;

    public float EntryTime { get; set; } = 0f;
    public float ExitTime { get; set; } = 0f;
    public bool ToggleAble { get; set; } = true;
}
