using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Platform.Storage;
using PngifyMe.Services;
using PngifyMe.ViewModels;

namespace PngifyMe.Views;

public partial class TITSSetup : UserControl
{
    public TITSSetup()
    {
        InitializeComponent();       

        DataContext = new TITSSetupViewModel(GetStorage);
    }

    private IStorageProvider GetStorage()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        return topLevel.StorageProvider;
    }
}