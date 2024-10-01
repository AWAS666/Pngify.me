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

public partial class MicStateSetup : UserControl
{
    public MicStateSetup()
    {
        InitializeComponent();
        DataContext = new MicroPhoneSetupViewModel(GetStorage);
        SettingsManager.Current.Profile.PropertyChanged += Profile_PropertyChanged;
    }

    private void Profile_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        DataContext = new MicroPhoneSetupViewModel(GetStorage);
    }

    private IStorageProvider GetStorage()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        return topLevel.StorageProvider;
    }

    private void CheckChanged(object? sender, RoutedEventArgs e)
    {
        MicroPhoneStateViewModel changed = (MicroPhoneStateViewModel)((CheckBox)e.Source).DataContext;
        //changed.DefaultState = !changed.DefaultState;
        var context = (MicroPhoneSetupViewModel)DataContext;
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
        var vm = (MicroPhoneStateViewModel)((Button)sender).DataContext;
        var options = new DrawerOptions()
        {
            Position = Position.Right,
            Buttons = DialogButton.OK,
            CanLightDismiss = true,
            IsCloseButtonVisible = false,
            Title = "Setup Transitions",

        };
        await Drawer.ShowCustomModal<TransitionView, MicroPhoneStateViewModel, object?>(vm, "LocalHost", options);
    }

    private void HotkeyDown(object sender, KeyEventArgs e)
    {
        var vm = ((TextBox)sender).DataContext as MicroPhoneStateViewModel;
        vm.OnKeyDown(e);
    }
}