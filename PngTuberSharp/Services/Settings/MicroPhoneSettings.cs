using System.Collections.Generic;

namespace PngTuberSharp.Services.Settings
{
    public class MicroPhoneSettings
    {
        public int ThreshHold { get; set; } = 50;
        public int Device { get; set; } = 0;
        public double BlinkInterval { get; set; } = 2f;
        public double BlinkTime { get; set; } = 0.25f;
        public List<MicroPhoneState> States { get; set; } = new List<MicroPhoneState>()
        {
            new MicroPhoneState()
            {
                Name = "Basic",
                Open = new ImageSetting()
                {
                    FilePath = "Assets/open.png",
                },
                 Closed = new ImageSetting()
                {
                    FilePath = "Assets/closed.png",
                },
            }
        };
    }

    public class MicroPhoneState
    {
        public string Name { get; set; }
        public ImageSetting Open { get; set; } = new();
        public ImageSetting OpenBlink { get; set; } = new();
        public ImageSetting Closed { get; set; } = new();
        public ImageSetting ClosedBlink { get; set; } = new();

        public HotkeyTrigger? Trigger { get; set; } = null;

    }
}
