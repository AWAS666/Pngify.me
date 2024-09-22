using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using NAudio.CoreAudioApi;
using PngifyMe.Services.Settings;
using PngifyMe.ViewModels;
using PngifyMe.ViewModels.Helper;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PngifyMe.Views.Helper;

public partial class TransitionView : UserControl
{
    public TransitionView()
    {
        InitializeComponent();
        //DataContext = new MicroPhoneStateViewModel();
    }    
}