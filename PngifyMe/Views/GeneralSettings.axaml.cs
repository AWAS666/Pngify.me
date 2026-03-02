using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using PngifyMe.Services;
using PngifyMe.Services.Twitch;
using PngifyMe.ViewModels;
using Semi.Avalonia;

namespace PngifyMe.Views;

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

        var spout = new Binding
        {
            Source = SettingsManager.Current.General,
            Path = nameof(SettingsManager.Current.General.EnableSpout),
            Mode = BindingMode.TwoWay
        };

        spout2.Bind(CheckBox.IsCheckedProperty, spout);

        var webOutputBinding = new Binding
        {
            Source = SettingsManager.Current.General,
            Path = nameof(SettingsManager.Current.General.EnableWebOutput),
            Mode = BindingMode.TwoWay
        };
        webOutput.Bind(CheckBox.IsCheckedProperty, webOutputBinding);

        SyncWebOutputBackgroundColorFromSetting();

        TwitchEventSocket.Authenticated += UpdateText;

        DataContext = new GeneralSettingsViewModel();

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

    private async void TwitchDeleteAuth(object sender, RoutedEventArgs e)
    {
        await TwitchEventSocket.DeleteAndReAuth();
    }

    private void SyncWebOutputBackgroundColorFromSetting()
    {
        var hex = SettingsManager.Current.General.WebOutputBackgroundColor ?? "#00FF00";
        hex = hex.TrimStart('#');
        if (hex.Length == 6 && int.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out _))
            webOutputBackgroundColorPicker.Color = Color.Parse($"#{hex}");
        else
            webOutputBackgroundColorPicker.Color = Color.Parse("#00FF00");
    }

    private void WebOutputBackgroundColorChanged(object? sender, ColorChangedEventArgs e)
    {
        var c = e.NewColor;
        SettingsManager.Current.General.WebOutputBackgroundColor = $"#{c.R:X2}{c.G:X2}{c.B:X2}";
    }
}