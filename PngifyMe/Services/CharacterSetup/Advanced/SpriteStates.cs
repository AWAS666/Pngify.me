using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Services.Settings;
using PngifyMe.ViewModels.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace PngifyMe.Services.CharacterSetup.Advanced;

public partial class SpriteStates : ObservableObject
{

    public string Name { get; set; }
    public int Index { get; set; }

    [ObservableProperty]
    private Trigger trigger;

    [property: JsonIgnore]
    [ObservableProperty]
    private TriggerViewModel triggerVm;

    public static List<TriggerTypeInfo> AvailableTriggers { get; } = new();
    static SpriteStates()
    {
        InitializeAvailableTriggers();
    }

    public SpriteStates()
    {
        Trigger = new HotkeyTrigger();
        TriggerVm = new TriggerViewModel(Trigger);
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
            TriggerVm = new TriggerViewModel(Trigger);
        }
    }

    partial void OnTriggerChanged(Trigger? oldValue, Trigger newValue)
    {
        SelectedTriggerType = AvailableTriggers.FirstOrDefault(t => t.Type == newValue?.GetType());
    }


}

public class TriggerTypeInfo
{
    public Type Type { get; set; }
    public string DisplayName { get; set; }
}
