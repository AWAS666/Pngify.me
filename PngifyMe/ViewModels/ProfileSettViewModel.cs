using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Layers;
using PngifyMe.Services;
using PngifyMe.Services.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;


namespace PngifyMe.ViewModels
{
    public partial class ProfileSettViewModel : ObservableObject
    {

        public ObservableCollection<ProfileViewModel> Profiles { get; set; }
        public ProfileSettings ProfilesSettings { get; set; }

        public ProfileSettViewModel()
        {
            ProfilesSettings = SettingsManager.Current.Profile;
            Profiles = new ObservableCollection<ProfileViewModel>(SettingsManager.Current.Profile.ProfileList.Select(x => new ProfileViewModel(x)));
        }

    }

    public partial class ProfileViewModel : ObservableObject
    {

        public Profile Profile { get; }

        private ProfileType type;

        public ProfileType Type
        {
            get => type; set
            {
                SetProperty(ref type, value);
                Profile.Type = value;
            }
        }

        public static List<ProfileType> ProfileTypes { get; set; } = new List<ProfileType>(Enum.GetValues(typeof(ProfileType)).Cast<ProfileType>());

        public ProfileViewModel(Profile profile)
        {
            Profile = profile;
            Type = profile.Type;
        }

    }
}
