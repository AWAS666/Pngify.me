using Avalonia.Controls;
using Avalonia.Data;
using PngTuberSharp.Services;
using PngTuberSharp.ViewModels;

namespace PngTuberSharp.Views;

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

        DataContext = new AvatarViewModel();
    }
}