using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PngTuberSharp.ViewModels;
using System;
using System.Linq;
using TwitchLib.Api.Helix.Models.Soundtrack;

namespace PngTuberSharp.Views;

public partial class MicStateSetup : UserControl
{
    public MicStateSetup()
    {
        InitializeComponent();
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
}