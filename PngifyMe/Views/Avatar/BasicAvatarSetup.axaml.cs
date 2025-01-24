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

namespace PngifyMe.Views.Avatar;

public partial class BasicAvatarSetup : UserControl
{
    public BasicAvatarSetup()
    {
        InitializeComponent();
        DataContext = new BasicSetupViewModel(GetStorage);
    }   

    private IStorageProvider GetStorage()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        return topLevel.StorageProvider;
    }

    private void CheckChanged(object? sender, RoutedEventArgs e)
    {
        BasicStateViewModel changed = (BasicStateViewModel)((CheckBox)e.Source).DataContext;
        //changed.DefaultState = !changed.DefaultState;
        var context = (BasicSetupViewModel)DataContext;
        if (changed.DefaultState)
        {
            foreach (var item in context.States.Where(x => x != changed))
            {
                item.DefaultState = false;
            }
        }
        else
        {
            context.States.First().DefaultState = true;
        }
        e.Handled = true;
    }

    private async void ShowTransition(object sender, RoutedEventArgs e)
    {
        var vm = (BasicStateViewModel)((Button)sender).DataContext;
        var options = new DrawerOptions()
        {
            Position = Position.Right,
            Buttons = DialogButton.OK,
            CanLightDismiss = true,
            IsCloseButtonVisible = false,
            Title = "Setup Transitions",

        };
        await Drawer.ShowCustomModal<TransitionView, BasicStateViewModel, object?>(vm, "LocalHost", options);
    }

    private void HotkeyDown(object sender, KeyEventArgs e)
    {
        var vm = ((TextBox)sender).DataContext as BasicStateViewModel;
        vm.OnKeyDown(e);
    }
}