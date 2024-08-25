namespace PngTuberSharp.Services.Settings
{
    public class TitsSettings
    {
        public bool Enabled { get; set; } = false;
        public bool HitLinesVisible { get; set; } = false;

        public bool ShowHitlines => Enabled && HitLinesVisible;
    }
}
