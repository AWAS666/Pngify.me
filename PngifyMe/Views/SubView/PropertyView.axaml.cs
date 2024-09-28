using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PngifyMe.Services.Settings;
using PngifyMe.ViewModels.Helper;
using System.Linq;
using System.Net;

namespace PngifyMe.Views.Helper;

public partial class PropertyView : UserControl
{
    public PropertyView()
    {
        InitializeComponent();
    }

    public async void SelectFile(object sender, RoutedEventArgs e)
    {
        var vm = (PropertyViewModel)((Button)e.Source).DataContext;
        var storage = TopLevel.GetTopLevel(this).StorageProvider;

        var path = await storage.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Select a File",
            FileTypeFilter = new[] { vm.PickFilter },
            AllowMultiple = false
        });

        if (!string.IsNullOrEmpty(path.FirstOrDefault()?.Path?.AbsolutePath))
        {
            vm.Value = WebUtility.UrlDecode(path.FirstOrDefault()?.Path?.AbsolutePath);
        }
    }


    public async void SelectFolder(object sender, RoutedEventArgs e)
    {
        var vm = (PropertyViewModel)((Button)e.Source).DataContext;
        var storage = TopLevel.GetTopLevel(this).StorageProvider;

        var path = await storage.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            Title = " Select a Folder"
        });      

        if (!string.IsNullOrEmpty(path.FirstOrDefault()?.Path?.AbsolutePath))
        {
            vm.Value = WebUtility.UrlDecode(path.FirstOrDefault()?.Path?.AbsolutePath);
        }
    }
}