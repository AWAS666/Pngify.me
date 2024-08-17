using PngTuberSharp.Settings;

namespace PngTuberSharp.Services.Settings
{
    public class AppSettings
    {
        public MicroPhoneSettings Microphone { get; set; } = new();
        public BackgroundSettings Background { get; set; } = new();
        public AvatarSettings Avatar { get; set; } = new();
        public LayerSettings LayerSetup { get; set; } = new();

    }
}
