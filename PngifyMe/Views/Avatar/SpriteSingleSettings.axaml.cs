using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PngifyMe.Services.CharacterSetup.Advanced;
using PngifyMe.ViewModels.Helper;
using System.Linq;
using System.Net;

namespace PngifyMe.Views.Avatar;

public partial class SpriteSingleSettings : UserControl
{
    public SpriteSingleSettings()
    {
        InitializeComponent();
    }

    private IStorageProvider GetStorage()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        return topLevel.StorageProvider;
    }

    private async void SwapImage(object sender, RoutedEventArgs e)
    {
        var con = (SpriteImage)DataContext;
        var storage = TopLevel.GetTopLevel(this).StorageProvider;
        var path = await storage.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Select an Image",
            FileTypeFilter = new[] { FilePickers.ImageAll },
            AllowMultiple = false
        });
        if (!string.IsNullOrEmpty(path.FirstOrDefault()?.Path?.AbsolutePath))
        {
            con.SwitchImage(WebUtility.UrlDecode(path.FirstOrDefault()?.Path?.AbsolutePath));
        }
    }
}