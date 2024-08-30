using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Threading;
using PngTuberSharp.Services;
using PngTuberSharp.Services.Twitch;

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

        //var bindingFpsTarget = new Binding
        //{
        //    Source = SettingsManager.Current.LayerSetup,
        //    Path = nameof(SettingsManager.Current.LayerSetup.TargetFPS),
        //    Mode = BindingMode.TwoWay
        //};

        //targetFPS.Bind(TextBox.TextProperty, bindingFpsTarget);

        TwitchEventSocket.Authenticated += UpdateText;

    }

    private void UpdateText(object? sender, TwitchAuth e)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            tokenValid.Text = e.Expiration.ToString();
        });
    }

    private async void TwitchRetry(object sender, RoutedEventArgs e)
    {
        await TwitchEventSocket.Start();
    }
}