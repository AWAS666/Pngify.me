using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using PngifyMe.Services;

namespace PngifyMe.Views;

public partial class VolumeSlider : UserControl
{
    public VolumeSlider()
    {
        InitializeComponent();
        MicrophoneService.LevelChanged += MicrophoneService_LevelChanged;
        max.Value = MicrophoneService.Settings.ThreshHold;
        max.ValueChanged += Max_ValueChanged;

        combo.ItemsSource = MicrophoneService.GetAllDevices();
        combo.SelectedIndex = MicrophoneService.Settings.Device;
        combo.SelectionChanged += Combo_SelectionChanged;
    }

    private void Combo_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        MicrophoneService.Settings.Device = combo.Items.IndexOf(e.AddedItems[0]);
        _ = MicrophoneService.Restart();
    }

    private void Max_ValueChanged(object? sender, Avalonia.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        MicrophoneService.Settings.ThreshHold = (int)e.NewValue;
    }

    private void MicrophoneService_LevelChanged(object? sender, MicroPhoneLevel e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            current.Value = e.Percent;
        });
    }

    private void SaveClick(object? sender, RoutedEventArgs e)
    {
        SettingsManager.Save();
    }
}