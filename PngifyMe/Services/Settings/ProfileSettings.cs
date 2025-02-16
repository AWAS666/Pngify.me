using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Helpers;
using PngifyMe.Layers;
using PngifyMe.Layers.Helper;
using PngifyMe.Services.CharacterSetup;
using PngifyMe.Services.CharacterSetup.Basic;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
            FixNames();
            string path = Path.Combine(SettingsManager.BasePath, "Profiles");
            // clear and create new
            Directory.Delete(path, true);
            Directory.CreateDirectory(path);
            foreach (var prof in ProfileList)
            {
                Directory.CreateDirectory(Path.Combine(path, prof.Name));
                File.WriteAllText(Path.Combine(path, prof.Name, "setup.json"), JsonSerializer.Serialize(prof, JsonSerializeHelper.GetDefault()));
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

        public async Task ExportProfile(Profile profile, string exportTo)
        {
            FixNames();
            await File.WriteAllTextAsync(exportTo, JsonSerializer.Serialize(profile, JsonSerializeHelper.GetDefault()));
        }

        public async Task<Profile> ImportProfile(string fromFile)
        {
            var profile = JsonSerializer.Deserialize<Profile>(await File.ReadAllTextAsync(fromFile), JsonSerializeHelper.GetDefault());

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
        public MicSettings MicSettings { get; set; } = new MicSettings();
        public IAvatarSettings AvatarSettings { get; set; } = new BasicCharSettings();

        public void SwitchType(ProfileType type)
        {
            if (Type == type) return;
            Type = type;
            AudioService.ChangeMode(type);
            if (Type == ProfileType.TTS)
                if (string.IsNullOrEmpty(SettingsManager.Current.LLM.OpenAIKey))
                    Log.Error("Missing openai key");
        }
    }

    public enum ProfileType
    {
        Human,
        TTS
    }
}
