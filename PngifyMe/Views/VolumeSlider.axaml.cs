using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using PngifyMe.Services;
using PngifyMe.ViewModels;
using System.Collections.Generic;

namespace PngifyMe.Views;

public partial class VolumeSlider : UserControl
{
    public VolumeSlider()
    {
        InitializeComponent();
        AudioService.LevelChanged += MicrophoneService_LevelChanged;       

        DataContext = new AudioSetupViewModel();
    }

    public List<string> Output { get; }

    private void InputDeviceChanged(object? sender, SelectionChangedEventArgs e)
    {
        _ = AudioService.Restart();
    }   

    private void MicrophoneService_LevelChanged(object? sender, MicroPhoneLevel e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            current.Value = e.Percent;
        });
    }  
}