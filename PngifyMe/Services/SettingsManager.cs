using PngifyMe.Helpers;
using PngifyMe.Layers.Helper;
using PngifyMe.Services.Settings;
using Serilog;
using System;
using System.IO;
using System.Text.Json;

namespace PngifyMe.Services
{
    public static class SettingsManager
    {
        public static string FilePath { get; }
        public static string BasePath => Specsmanager.BasePath;

        public static AppSettings Current { get; private set; }

        static SettingsManager()
        {

#if DEBUG
            FilePath = Path.Join(Specsmanager.BasePath, "settings.dev.json");
#else
            FilePath = Path.Join(Specsmanager.BasePath, "settings.json");
#endif
            Load();
        }

        public static void Load()
        {
            if (File.Exists(FilePath))
            {
                try
                {
                    Current = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(FilePath), JsonSerializeHelper.GetDefault());
                }
                catch (Exception e)
                {
                    Current = new();
                    Save();
                    Log.Error(e.Message);
                }
            }
            else
            {
                Current = new();
                Save();
            }
            Current.Profile.Load();
        }

        public static void Save()
        {
            File.WriteAllText(FilePath, JsonSerializer.Serialize(Current, JsonSerializeHelper.GetDefault()));
        }
    }
}
