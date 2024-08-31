using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Platform.Storage;
using PngTuberSharp.Services;
using PngTuberSharp.ViewModels;

namespace PngTuberSharp.Views;

public partial class TITSSetup : UserControl
{
    public TITSSetup()
    {
        InitializeComponent();

        var binding = new Binding
        {
            Source = SettingsManager.Current.Tits,
            Path = nameof(SettingsManager.Current.Tits.Enabled),
            Mode = BindingMode.TwoWay
        };
        activated.Bind(CheckBox.IsCheckedProperty, binding);

        var bindingHit = new Binding
        {
            Source = SettingsManager.Current.Tits,
            Path = nameof(SettingsManager.Current.Tits.HitLinesVisible),
            Mode = BindingMode.TwoWay
        };
        showHitbox.Bind(CheckBox.IsCheckedProperty, bindingHit);

        DataContext = new TITSSetupViewModel(GetStorage);
    }

    private IStorageProvider GetStorage()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        return topLevel.StorageProvider;
    }
}