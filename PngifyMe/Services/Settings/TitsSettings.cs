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
        private float volume = 0.5f;

        [ObservableProperty]
        private uint collissionEnergyLossPercent = 20;

        public TitsTriggerSetup ThrowSetup { get; set; } = new();
        public TitsTriggerSetup RainSetup { get; set; } = new();

        public bool ShowHitlines => Enabled && HitLinesVisible;
    }

    public partial class TitsTriggerSetup : ObservableObject
    {
        [ObservableProperty]
        private uint objectSpeedMin = 2000;

        [ObservableProperty]
        private uint objectSpeedMax = 3000;

        [ObservableProperty]
        private uint minBits = 0;

        [ObservableProperty]
        private uint maxBits = uint.MaxValue;

        [ObservableProperty]
        private string redeem = string.Empty;
    }
}
