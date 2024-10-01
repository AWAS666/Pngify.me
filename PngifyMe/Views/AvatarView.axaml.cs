using Avalonia.Controls;
using Avalonia.Data;
using PngifyMe.Services;
using PngifyMe.ViewModels;

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
        DataContext = vm;
    }
}