using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using PngTuberSharp.Services;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace PngTuberSharp.Views;

public partial class ChangeAvatar : UserControl
{
    public ChangeAvatar()
    {
        InitializeComponent();
        RefreshPaths();
    }

    private void RefreshPaths()
    {
        fileOpen.Text = Path.GetFileName(SettingsManager.Current.Avatar.Open);
        fileClosed.Text = Path.GetFileName(SettingsManager.Current.Avatar.Closed);
    }

    private async void ChangeOpen(object sender, RoutedEventArgs args)
    {
        var filePath = await GetImageFile();
        if (!string.IsNullOrEmpty(filePath))
        {
            SettingsManager.Current.Avatar.Open = filePath;
            SettingsManager.Current.Avatar.Refresh?.Invoke(null, null);
            RefreshPaths();
        }
    }

    private async void ChangeClosed(object sender, RoutedEventArgs args)
    {
        var filePath = await GetImageFile();
        if (!string.IsNullOrEmpty(filePath))
        {
            SettingsManager.Current.Avatar.Closed = filePath;
            SettingsManager.Current.Avatar.Refresh?.Invoke(null, null);
            RefreshPaths();
        }
    }

    private async Task<string> GetImageFile()
    {
        // Create the open file dialog
        var openFileDialog = new OpenFileDialog
        {
            Title = "Select an Image",
            Filters = new()
                {
                    new FileDialogFilter { Name = "Image Files", Extensions = { "jpg", "jpeg", "png", "bmp", "gif" } }
                },
            AllowMultiple = false
        };

        // Open the dialog and get the file path
        var result = await openFileDialog.ShowAsync(GetWindow());

        // Check if a file was selected
        if (result != null && result.Length > 0)
        {
            string filePath = result[0];
            return filePath;
        }
        return null;
    }

    private Window? GetWindow()
    {
        // Traverse the logical tree to find the closest window
        var window = this.FindAncestorOfType<Window>();
        return window;
    }
}