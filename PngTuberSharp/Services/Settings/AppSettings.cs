using PngTuberSharp.Settings;

namespace PngTuberSharp.Services.Settings
{
    public class AppSettings
    {
        public GeneralSettings General { get; set; } = new();
        public MicroPhoneSettings Microphone { get; set; } = new();
        public BackgroundSettings Background { get; set; } = new();
        public AvatarSettings Avatar { get; set; } = new();
        public LayerSetup LayerSetup { get; set; } = new();
        public TwitchSettings Twitch { get; set; } = new();
        public TitsSettings Tits { get; set; } = new();

    }
}
