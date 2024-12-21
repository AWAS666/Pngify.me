using Avalonia.Controls;
using Avalonia.Interactivity;
using PngifyMe.Services;
using PngifyMe.Services.Settings;
using PngifyMe.ViewModels;
using System.Linq;

namespace PngifyMe.Views;

public partial class ProfileSetup : UserControl
{
    public ProfileSetup()
    {
        InitializeComponent();
        DataContext = new ProfileSettViewModel();
    }


    public void LoadProfile(object sender, RoutedEventArgs e)
    {
        var vm = (ProfileViewModel)((Button)e.Source).DataContext;
        SettingsManager.Current.Profile.LoadNewProfile(vm.Profile);
    }

    public void DeleteProfile(object sender, RoutedEventArgs e)
    {
        var vmS = (ProfileViewModel)((Button)e.Source).DataContext;
        var vm = (ProfileSettViewModel)DataContext;
        if (!vmS.Profile.Default)
        {
            SettingsManager.Current.Profile.Delete(vmS.Profile);
            vm.Profiles.Remove(vmS);
        }
    }

    public void CreateNewProfile(object sender, RoutedEventArgs e)
    {
        string text = newProfile.Text;
        if (string.IsNullOrEmpty(text))
            return;
        var vm = (ProfileSettViewModel)DataContext;
        if (vm.Profiles.Any(x => x.Profile.Name == text))
        {
            newProfile.Text = string.Empty;
            return;
        }
        var profile = new Profile() { Name = text };
        vm.Profiles.Add(new ProfileViewModel(profile));
        SettingsManager.Current.Profile.ProfileList.Add(profile);
    }

    private void CheckChanged(object? sender, RoutedEventArgs e)
    {
        var changed = (ProfileViewModel)((CheckBox)e.Source).DataContext;
        var context = (ProfileSettViewModel)DataContext;
        if (changed.DefaultValue)
        {
            foreach (var item in context.Profiles.Where(x => x != changed))
            {
                item.DefaultValue = false;
            }
        }
        else
        {
            context.Profiles.First().DefaultValue = true;
        }
        e.Handled = true;
    }


    public async void Export(object sender, RoutedEventArgs e)
    {
        var vmS = (ProfileViewModel)((Button)e.Source).DataContext;
        var vm = (ProfileSettViewModel)DataContext;
        var top = TopLevel.GetTopLevel(this);

        var path = await top.StorageProvider.SaveFilePickerAsync(new Avalonia.Platform.Storage.FilePickerSaveOptions()
        {
            SuggestedFileName = $"{vmS.Profile.Name}.zip"
        });

        SettingsManager.Current.Profile.ExportProfile(vmS.Profile, path.Path.AbsolutePath);
    }

    public async void Import(object sender, RoutedEventArgs e)
    {
        var vm = (ProfileSettViewModel)DataContext;
        var top = TopLevel.GetTopLevel(this);

        var path = await top.StorageProvider.OpenFilePickerAsync(new Avalonia.Platform.Storage.FilePickerOpenOptions()
        {
            // todo: add file filter
        });

        foreach (var item in path)
        {
            var prof = SettingsManager.Current.Profile.ImportProfile(item.Path.AbsolutePath);
            vm.Profiles.Add(new ProfileViewModel(prof));
        }
    }
}