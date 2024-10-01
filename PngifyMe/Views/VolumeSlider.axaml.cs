using Avalonia.Controls;
using Avalonia.Threading;
using PngifyMe.Services;
using PngifyMe.ViewModels;
using System.Linq;

namespace PngifyMe.Views;

public partial class VolumeSlider : UserControl
{
    public VolumeSlider()
    {
        InitializeComponent();
        AudioService.LevelChanged += MicrophoneService_LevelChanged;

        var vm = new AudioSetupViewModel();
        DataContext = vm;
        inputAudio.SelectedValue = AudioService.InputDevices.FirstOrDefault(x => x.Id == vm.Settings.DeviceIn);
        outputAudio.SelectedValue = AudioService.OutputDevices.FirstOrDefault(x => x.Id == vm.Settings.DeviceOut);
    }

    private void InputDeviceChanged(object? sender, SelectionChangedEventArgs e)
    {
        var vm = (AudioSetupViewModel)DataContext;
        if (e.AddedItems.Count == 1)
        {
            vm.Settings.DeviceIn = ((AudioDeviceConfig)e.AddedItems[0]).Id;
        }
        AudioService.Restart();
    }

    private void OutputDeviceChanged(object? sender, SelectionChangedEventArgs e)
    {
        var vm = (AudioSetupViewModel)DataContext;
        if (e.AddedItems.Count == 1)
        {
            vm.Settings.DeviceOut = ((AudioDeviceConfig)e.AddedItems[0]).Id;
        }
    }



    private void MicrophoneService_LevelChanged(object? sender, MicroPhoneLevel e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            current.Value = e.Percent;
        });
    }
}