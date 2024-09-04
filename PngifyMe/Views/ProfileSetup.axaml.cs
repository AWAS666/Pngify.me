using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using PngifyMe.Services;
using PngifyMe.Services.Settings;
using PngifyMe.ViewModels;

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

    public void CreateNewProfile(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(newProfile.Text))
            return;
        var vm = (ProfileSettViewModel)DataContext;
        var profile = new Profile() { Name = newProfile.Text };
        vm.Profiles.Add(new ProfileViewModel(profile));
        SettingsManager.Current.Profile.ProfileList.Add(profile);
    }
}