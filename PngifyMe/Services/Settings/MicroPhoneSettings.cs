using CommunityToolkit.Mvvm.ComponentModel;
using PortAudioSharp;
using System.Collections.Generic;

namespace PngifyMe.Services.Settings
{
    public partial class MicroPhoneSettings : ObservableObject
    {
        [ObservableProperty]
        private int threshHold = 25;

        [ObservableProperty]
        private int deviceIn = PortAudio.DefaultInputDevice;     
           
        [ObservableProperty]
        private int deviceOut = PortAudio.DefaultOutputDevice;

        [ObservableProperty]
        private int smoothing = 5;

        [ObservableProperty]
        private double blinkInterval = 2f;

        [ObservableProperty]
        private double blinkTime = 0.25f;

        [ObservableProperty]
        private double transitionTime = 0.15f;

        public List<MicroPhoneState> States { get; set; } = new List<MicroPhoneState>()
        {
            new MicroPhoneState()
            {
                Name = "Basic",
                Default = true,
                Open = new ImageSetting()
                {
                    FilePath = "Assets/openMouthopenEyes.png",
                },
                 Closed = new ImageSetting()
                {
                    FilePath = "Assets/closedMouthopenEyes.png",
                },
                 ClosedBlink = new ImageSetting()
                 {
                     FilePath = "Assets/closedMouthclosedEyes.png"
                 },
                 OpenBlink = new ImageSetting()
                 {
                     FilePath = "Assets/openMouthclosedEyes.png"
                 }
            }
        };
    }

    public class MicroPhoneState
    {
        public string Name { get; set; }
        public bool Default { get; set; }
        public ImageSetting Open { get; set; } = new();
        public ImageSetting OpenBlink { get; set; } = new();
        public ImageSetting Closed { get; set; } = new();
        public ImageSetting ClosedBlink { get; set; } = new();

        public HotkeyTrigger? Trigger { get; set; } = null;

    }
}
