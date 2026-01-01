using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Services.CharacterSetup.Images;
using PngifyMe.Services.Settings;
using PngifyMe.ViewModels.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace PngifyMe.Services.CharacterSetup.Basic;

public partial class CharacterState : ObservableObject
{
    public string Name { get; set; }
    public bool Default { get; set; }
    public ImageSetting Open { get; set; } = new();
    public ImageSetting OpenBlink { get; set; } = new();
    public ImageSetting Closed { get; set; } = new();
    public ImageSetting ClosedBlink { get; set; } = new();
    public ImageSetting EntryImage { get; set; } = new();
    public ImageSetting ExitImage { get; set; } = new();

    [ObservableProperty]
    private Trigger trigger;

    [property: JsonIgnore]
    [ObservableProperty]
    private TriggerViewModel triggerVm;

    public static List<TriggerTypeInfo> AvailableTriggers { get; } = new();
    static CharacterState()
    {
        InitializeAvailableTriggers();
    }

    public CharacterState()
    {
        Trigger = new HotkeyTrigger();
        TriggerVm = new TriggerViewModel(Trigger);
        SelectedTriggerType = AvailableTriggers.FirstOrDefault(t => t.Type == Trigger.GetType());
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

    public float EntryTime { get; set; } = 0f;
    public float ExitTime { get; set; } = 0f;
    public bool ToggleAble { get; set; } = true;
}
