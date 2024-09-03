using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using NAudio.CoreAudioApi;
using PngifyMe.Services;
using PngifyMe.ViewModels;
using PngifyMe.ViewModels.Helper;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PngifyMe.Views;

public partial class LayerSetup : UserControl
{
    public LayerSetup()
    {
        InitializeComponent();
        DataContext = new LayerSetupViewModel(SettingsManager.Current.LayerSetup.Layers);
    }

    public async void SelectFile(object sender, RoutedEventArgs e)
    {
        var vm = (PropertyViewModel)((Button)e.Source).DataContext;
        var storage = TopLevel.GetTopLevel(this).StorageProvider;

        var path = await storage.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Select an Image",
            FileTypeFilter = new[] { FilePickers.ImageAll },
            AllowMultiple = false
        });

        if (!string.IsNullOrEmpty(path.FirstOrDefault()?.Path?.AbsolutePath))
        {
            vm.Value = WebUtility.UrlDecode(path.FirstOrDefault()?.Path?.AbsolutePath);
        }
    }

}