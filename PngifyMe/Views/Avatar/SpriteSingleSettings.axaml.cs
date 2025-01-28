using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PngifyMe.Services.CharacterSetup.Advanced;

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

    private void SwapImage(object sender, RoutedEventArgs e)
    {
        var con = (SpriteImage)DataContext;
        string path = ""; // todo load path
        con.SwitchImage(path);
    }
}