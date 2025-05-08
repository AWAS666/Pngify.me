using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Services.CharacterSetup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngifyMe.Services.Settings;

public partial class MicSettings : ObservableObject
{
    [ObservableProperty]
    private int threshHold = 25;

    [ObservableProperty]
    private int deviceIn = 0;

    [ObservableProperty]
    private int deviceOut = 0;

    [ObservableProperty]
    private int smoothing = 20;

    public EventHandler<int> DeviceOutChanged;

    partial void OnDeviceOutChanged(int oldValue, int newValue)
    {
        DeviceOutChanged?.Invoke(this, newValue);
    }
}
