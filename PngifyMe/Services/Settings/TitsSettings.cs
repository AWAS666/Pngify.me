using CommunityToolkit.Mvvm.ComponentModel;

namespace PngifyMe.Services.Settings
{
    public partial class TitsSettings : ObservableObject
    {
        public bool Enabled { get; set; } = false;
        public bool HitLinesVisible { get; set; } = false;

        [ObservableProperty]
        private uint gravity = 1000;

        [ObservableProperty]
        private string hitSound = string.Empty;

        [ObservableProperty]
        private string hitSoundFileName = string.Empty;

        [ObservableProperty]
        private bool enableSound = true;

        [ObservableProperty]
        private uint objectSpeedMin = 2000;

        [ObservableProperty]
        private uint objectSpeedMax = 3000;

        [ObservableProperty]
        private uint collissionEnergyLossPercent = 20;

        public bool ShowHitlines => Enabled && HitLinesVisible;
    }
}
