using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Services.CharacterSetup;
using PortAudioSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngifyMe.Services.Settings;

public partial class MicSettings : ObservableObject
{
    [ObservableProperty]
    private int threshHold = 25;

    [ObservableProperty]
    private int deviceIn = PortAudio.DefaultInputDevice;

    [ObservableProperty]
    private int deviceOut = PortAudio.DefaultOutputDevice;

    [ObservableProperty]
    private int smoothing = 20;
}
