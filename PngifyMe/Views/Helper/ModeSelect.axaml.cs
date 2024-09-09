using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using PngifyMe.Services;
using PngifyMe.Services.Settings;
using PngifyMe.ViewModels.Helper;
using System.Linq;
using System.Net;
using System.Security.Cryptography;

namespace PngifyMe.Views.Helper;

public partial class ModeSelect : UserControl
{
    public ModeSelect()
    {
        InitializeComponent();
        Recolour();
    }

    private void Recolour()
    {
        switch (SettingsManager.Current.Profile.Active.Type)
        {
            case ProfileType.Human:
                human.Background = Brushes.Green;
                tts.Background = Brushes.LightSlateGray;
                break;
            case ProfileType.TTS:
                tts.Background = Brushes.Green;
                human.Background = Brushes.LightSlateGray;
                break;
        }
    }

    private void SwitchToHuman(object sender, RoutedEventArgs e)
    {
        SettingsManager.Current.Profile.Active.SwitchType(ProfileType.Human);
        Recolour();
    }

    private void SwitchToTTS(object sender, RoutedEventArgs e)
    {
        SettingsManager.Current.Profile.Active.SwitchType(ProfileType.TTS);
        Recolour();
    }
}