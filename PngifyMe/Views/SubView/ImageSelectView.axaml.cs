using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using NAudio.CoreAudioApi;
using PngifyMe.Services.Settings;
using PngifyMe.ViewModels.Helper;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PngifyMe.Views.Helper;

public partial class ImageSelectView : UserControl
{
    public ImageSelectView()
    {
        InitializeComponent();
        DataContext = new ImageSetting();
    }

    public async void LoadFile(object sender, RoutedEventArgs e)
    {
        var vm = (ImageSetting)DataContext;
        var storage = TopLevel.GetTopLevel(this).StorageProvider;
        var path = await storage.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Select an Image",
            FileTypeFilter = new[] { FilePickers.ImageAll },
            AllowMultiple = false
        });
        if (!string.IsNullOrEmpty(path.FirstOrDefault()?.Path?.AbsolutePath))
        {
            vm.FilePath = WebUtility.UrlDecode(path.FirstOrDefault()?.Path?.AbsolutePath);
        }
    }

    public void Delete(object sender, RoutedEventArgs e)
    {
        var vm = (ImageSetting)DataContext;
        vm.FilePath = string.Empty;
    }
}