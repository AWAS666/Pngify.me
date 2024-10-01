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
        public static string BasePath { get; }

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

                    var options = new JsonSerializerOptions();
                    options.Converters.Add(new BaseLayerJsonConverter());
                    options.Converters.Add(new TriggerJsonConverter());
                    Current = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(FilePath), options);
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
            var options = new JsonSerializerOptions();
            options.Converters.Add(new BaseLayerJsonConverter());
            options.Converters.Add(new TriggerJsonConverter());
#if DEBUG
            options.WriteIndented = true;
#endif
            File.WriteAllText(FilePath, JsonSerializer.Serialize(Current, options));
        }
    }
}
