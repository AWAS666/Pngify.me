using Avalonia.Controls;
using Avalonia.Data;
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
}