using PngifyMe.Layers.Helper;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace PngifyMe.Services.Settings
{
    public class ProfileSettings
    {
        // json ignore this, this loads based on folders
        public List<Profile> ProfileList { get; set; } = new List<Profile>();

        public Profile Active { get; set; }

        public ProfileSettings()
        {
            Load();
        }

        public void Load()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new BaseLayerJsonConverter());
            options.Converters.Add(new TriggerJsonConverter());

            string path = Path.Combine(SettingsManager.BasePath, "Profiles");
            Directory.CreateDirectory(path);
            foreach (var dir in Directory.GetDirectories(path))
            {
                var prof = JsonSerializer.Deserialize<Profile>(File.ReadAllText(Path.Combine(dir, "setup.json")), options);
                ProfileList.Add(prof);
            }

            if (ProfileList.Count == 0)
            {
                ProfileList.Add(new Profile() { Default = true });
                Save();
            }
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
            }
        }

        public void SetDefault(Profile profile)
        {
            ProfileList.ForEach(x => x.Default = false);
            profile.Default = true;
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
    }

    public class Profile
    {
        public string Name { get; set; } = "Profile1";
        public bool Default { get; set; }
        public ProfileType Type { get; set; } = ProfileType.Human;
        public MicroPhoneSettings MicrophoneSettings { get; set; } = new();
    }

    public enum ProfileType
    {
        Human,
        TTS
    }
}
