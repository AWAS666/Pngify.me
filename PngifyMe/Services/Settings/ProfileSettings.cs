using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Layers.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PngifyMe.Services.Settings
{
    public partial class ProfileSettings : ObservableObject
    {
        // json ignore this, this loads based on folders
        // load this instead of json
        public List<Profile> ProfileList { get; set; } = [
            new Profile()
            {
                Name= "Default",
                Default = true
            }
        ];

        [ObservableProperty]
        [property: JsonIgnore]
        private Profile active;

        public ProfileSettings()
        {
        }

        public void Load()
        {           
            Active = ProfileList.First(x => x.Default);
        }

        public void Save()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new BaseLayerJsonConverter());
            options.Converters.Add(new TriggerJsonConverter());

            FixNames();
            string path = Path.Combine(SettingsManager.BasePath, "Profiles");
            // clear and create new
            Directory.Delete(path, true);
            Directory.CreateDirectory(path);
            foreach (var prof in ProfileList)
            {
                Directory.CreateDirectory(Path.Combine(path, prof.Name));
                File.WriteAllText(Path.Combine(path, prof.Name, "setup.json"), JsonSerializer.Serialize(prof, options));
                // move and save all images
                // make it a zip file
            }
        }

        public void SetDefault(Profile profile)
        {
            ProfileList.ForEach(x => x.Default = false);
            profile.Default = true;
        }

        public void LoadNewProfile(Profile profile)
        {
            Active = profile;
        }

        private void FixNames()
        {
            List<string> names = new List<string>();
            foreach (var item in ProfileList)
            {
                while (names.Contains(item.Name))
                {
                    item.Name += "_1";
                }
                names.Add(item.Name);
            }
        }

        public void Delete(Profile profile)
        {
            // active default if currently active
            if (Active == profile)
                Active = ProfileList.First(x => x.Default);
            ProfileList.Remove(profile);
        }

        public void ExportProfile(Profile profile, string exportTo)
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new BaseLayerJsonConverter());
            options.Converters.Add(new TriggerJsonConverter());

            FixNames();
            string path = Path.Combine(SettingsManager.BasePath, "Profiles");
            // clear and create new
            Directory.CreateDirectory(path);

            var newFolder = Path.Combine(path, profile.Name);
            Directory.CreateDirectory(newFolder);

            // move and save all images
            foreach (var item in profile.MicroPhone.States)
            {
                item.Open.FilePath = MoveFile(item.Open.FilePath, newFolder);
                item.Closed.FilePath = MoveFile(item.Closed.FilePath, newFolder);
                item.OpenBlink.FilePath = MoveFile(item.OpenBlink.FilePath, newFolder);
                item.ClosedBlink.FilePath = MoveFile(item.ClosedBlink.FilePath, newFolder);
            }
            File.WriteAllText(Path.Combine(newFolder, "setup.json"), JsonSerializer.Serialize(profile, options));

            // make it a zip file
            ZipFile.CreateFromDirectory(newFolder, exportTo);
        }

        public Profile ImportProfile(string fromFile)
        {
            string path = Path.Combine(SettingsManager.BasePath, "Profiles");

            string output = Path.Combine(path, Path.GetFileNameWithoutExtension(fromFile));
            Directory.CreateDirectory(output);
            ZipFile.ExtractToDirectory(fromFile, output, true);
            var profile = JsonSerializer.Deserialize<Profile>(File.ReadAllText(Path.Combine(output, "setup.json")));

            // fix any broken image references
            foreach (var item in profile.MicroPhone.States)
            {
                item.Open.FilePath = Path.Combine(output, Path.GetFileNameWithoutExtension(item.Open.FilePath));
                item.Closed.FilePath = Path.Combine(output, Path.GetFileNameWithoutExtension(item.Closed.FilePath));

                if (item.ClosedBlink.FilePath != null)
                    item.ClosedBlink.FilePath = Path.Combine(output, Path.GetFileNameWithoutExtension(item.ClosedBlink.FilePath));
                if (item.OpenBlink.FilePath != null)
                    item.OpenBlink.FilePath = Path.Combine(output, Path.GetFileNameWithoutExtension(item.OpenBlink.FilePath));
            }
            profile.Default = false;

            ProfileList.Add(profile);
            return profile;
        }

        private string? MoveFile(string? filePath, string folder)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return null;

            var name = Path.GetFileName(filePath);
            var newPath = Path.Combine(folder, name);

            File.Copy(filePath, newPath, true);

            return newPath;
        }
    }

    public class Profile
    {
        public string Name { get; set; } = "Profile1";
        public bool Default { get; set; }
        public ProfileType Type { get; set; } = ProfileType.Human;
        public MicroPhoneSettings MicroPhone { get; set; } = new();

        public void SwitchType(ProfileType type)
        {
            if (Type == type) return;
            Type = type;
            AudioService.ChangeMode(type);
        }
    }

    public enum ProfileType
    {
        Human,
        TTS
    }
}
