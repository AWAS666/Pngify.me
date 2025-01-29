using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PngifyMe.Layers;
using PngifyMe.Services;
using PngifyMe.Services.CharacterSetup.Advanced;
using PngifyMe.ViewModels;
using PngifyMe.ViewModels.Helper;
using PngifyMe.Views.Helper;
using System.ComponentModel;
using System.Linq;
using Ursa.Common;
using Ursa.Controls;
using Ursa.Controls.Options;

namespace PngifyMe.Views.Avatar;

public partial class SpriteAvatarSetup : UserControl
{
    public SpriteAvatarSetup()
    {
        InitializeComponent();
    }

    private IStorageProvider GetStorage()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        return topLevel.StorageProvider;
    }

    private void ActivateState(object sender, RoutedEventArgs e)
    {
        var vm = (SpriteStates)((Button)sender).DataContext;
        LayerManager.CharacterStateHandler.ToggleState(vm.Name);
    }
}