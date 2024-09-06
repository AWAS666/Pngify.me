using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using NAudio.CoreAudioApi;
using PngifyMe.Services;
using PngifyMe.ViewModels;
using PngifyMe.ViewModels.Helper;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PngifyMe.Views;

public partial class LayerSetup : UserControl
{
    public LayerSetup()
    {
        InitializeComponent();
        DataContext = new LayerSetupViewModel(SettingsManager.Current.LayerSetup.Layers);
    }

   

}