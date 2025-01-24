using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PngifyMe.Services;
using PngifyMe.ViewModels;
using PngifyMe.ViewModels.Helper;
using PngifyMe.Views.Helper;
using System.ComponentModel;
using System.Linq;
using Ursa.Common;
using Ursa.Controls;
using Ursa.Controls.Options;

namespace PngifyMe.Views;

public partial class AvatarSetup : UserControl
{
    public AvatarSetup()
    {
        InitializeComponent();
        DataContext = new AvatarSetupViewModel(GetStorage);
    }  

    private IStorageProvider GetStorage()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        return topLevel.StorageProvider;
    } 
}