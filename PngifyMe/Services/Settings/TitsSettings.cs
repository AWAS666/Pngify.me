using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.ViewModels.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;


namespace PngifyMe.Services.Settings;

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

    [ObservableProperty]
    private bool useTwitchEmotes = false;

    [ObservableProperty]
    private bool useFolderEmotes = false;

    [ObservableProperty]
    private uint throwSize = 75;

    partial void OnUseTwitchEmotesChanged(bool oldValue, bool newValue)
    {
        if (interlock) return;
        interlock = true;
        if (newValue)
            UseFolderEmotes = false;
        ThrowEmotesChanged?.Invoke(this, EventArgs.Empty);
        interlock = false;
    }

    partial void OnUseFolderEmotesChanged(bool oldValue, bool newValue)
    {
        if (interlock) return;
        interlock = true;
        if (newValue)
            UseTwitchEmotes = false;
        ThrowEmotesChanged?.Invoke(this, EventArgs.Empty);
        interlock = false;
    }

    partial void OnThrowSizeChanged(uint oldValue, uint newValue)
    {
        ThrowEmotesChanged?.Invoke(this, EventArgs.Empty);
    }

    public EventHandler ThrowEmotesChanged;
    private bool interlock;

    public TitsTriggerSetup ThrowSetup { get; set; } = new();
    public TitsTriggerSetup RainSetup { get; set; } = new();

    public ObservableCollection<TitsCustomTrigger> CustomTriggers { get; set; } = new();

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

public partial class TitsCustomTrigger : ObservableObject
{
    public string Name { get; set; }
    public int BitsToThrow { get; set; } = 10;
    public bool UseRain { get; set; }

    [ObservableProperty]
    private Trigger trigger;

    [property: JsonIgnore]
    [ObservableProperty]
    private TriggerViewModel triggerVm;

    public static List<TriggerTypeInfo> AvailableTriggers { get; } = new();
    static TitsCustomTrigger()
    {
        InitializeAvailableTriggers();
    }

    public TitsCustomTrigger()
    {
        Trigger = new HotkeyTrigger();
    }

    private static void InitializeAvailableTriggers()
    {
        var triggerTypes = typeof(Trigger).Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(Trigger)) && !t.IsAbstract);


        foreach (var type in triggerTypes)
        {
            if (type == typeof(AlwaysActive)) continue;
            AvailableTriggers.Add(new TriggerTypeInfo
            {
                Type = type,
                DisplayName = type.Name
            });
        }
    }

    [property: JsonIgnore]
    [ObservableProperty]
    private TriggerTypeInfo _selectedTriggerType;

    partial void OnSelectedTriggerTypeChanged(TriggerTypeInfo? oldValue, TriggerTypeInfo newValue)
    {
        if (newValue != null && (Trigger?.GetType() != newValue.Type))
        {
            Trigger = (Trigger)Activator.CreateInstance(newValue.Type)!;
        }
    }

    partial void OnTriggerChanged(Trigger? oldValue, Trigger newValue)
    {
        SelectedTriggerType = AvailableTriggers.FirstOrDefault(t => t.Type == newValue?.GetType());
        TriggerVm = new TriggerViewModel(newValue);
    }


}

public class TriggerTypeInfo
{
    public Type Type { get; set; }
    public string DisplayName { get; set; }
}
