using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Threading;
using PngTuberSharp.Services;
using PngTuberSharp.Services.Twitch;
using System.Threading.Tasks;

namespace PngTuberSharp.Views;

public partial class GeneralSettings : UserControl
{
    public GeneralSettings()
    {
        InitializeComponent();

        var binding = new Binding
        {
            Source = SettingsManager.Current.Twitch,
            Path = nameof(SettingsManager.Current.Twitch.Enabled),
            Mode = BindingMode.TwoWay
        };

        twitchEnabled.Bind(CheckBox.IsCheckedProperty, binding);

        var bindingFps = new Binding
        {
            Source = SettingsManager.Current.LayerSetup,
            Path = nameof(SettingsManager.Current.LayerSetup.ShowFPS),
            Mode = BindingMode.TwoWay
        };

        fps.Bind(CheckBox.IsCheckedProperty, bindingFps);

        var bindingFpsTarget = new Binding
        {
            Source = SettingsManager.Current.LayerSetup,
            Path = nameof(SettingsManager.Current.LayerSetup.TargetFPS),
            Mode = BindingMode.TwoWay
        };

        targetFPS.Bind(TextBox.TextProperty, bindingFpsTarget);

    }

    private async void TwitchRetry(object sender, RoutedEventArgs e)
    {
        await TwitchEventSocket.Start();
    }
}