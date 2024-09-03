using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Threading;
using PngifyMe.Services;
using PngifyMe.ViewModels;
using System;

namespace PngifyMe.Views;

public partial class AvatarView : UserControl
{
    public AvatarView()
    {
        InitializeComponent();

        var bindingFps = new Binding
        {
            Source = SettingsManager.Current.LayerSetup,
            Path = nameof(SettingsManager.Current.LayerSetup.ShowFPS),
            Mode = BindingMode.TwoWay
        };

        fpsCounter.Bind(TextBlock.IsVisibleProperty, bindingFps);

        var vm = new AvatarViewModel();
        vm.RequestRedraw += RedrawVisual;
        DataContext = vm;
    }

    private void RedrawVisual(object? sender, EventArgs e)
    {
        Dispatcher.UIThread.Post(image.InvalidateVisual);
    }
}