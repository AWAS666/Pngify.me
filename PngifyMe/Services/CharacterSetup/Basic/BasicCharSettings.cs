using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Services.CharacterSetup.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngifyMe.Services.CharacterSetup.Basic;

public partial class BasicCharSettings : ObservableObject, IAvatarSettings
{
    [ObservableProperty]
    private double blinkInterval = 2f;

    [ObservableProperty]
    private double blinkTime = 0.25f;

    [ObservableProperty]
    private double transitionTime = 0.15f;
    public List<CharacterState> States { get; set; } = new List<CharacterState>()
    {
        new CharacterState()
        {
            Name = "Basic",
            Default = true,
            Open = new ImageSetting().LoadFromFile("Assets/openMouthopenEyes.png"),
            Closed = new ImageSetting().LoadFromFile("Assets/closedMouthopenEyes.png"),
            ClosedBlink = new ImageSetting().LoadFromFile("Assets/closedMouthclosedEyes.png"),
            OpenBlink = new ImageSetting().LoadFromFile("Assets/openMouthclosedEyes.png")
           
        }
    };

    public List<string> AvailableStates() => States.Select(s => s.Name).ToList();
}
