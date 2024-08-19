using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using PngTuberSharp.Services;
using PngTuberSharp.ViewModels;

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
}