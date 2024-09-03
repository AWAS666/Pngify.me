using PngifyMe.Layers;
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

        public static AppSettings Current { get; private set; }

        static SettingsManager()
        {
            // load settings
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder) + "\\PngifyMe";
            Directory.CreateDirectory(path);
            FilePath = Path.Join(path, "settings.json");
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
        }

        public static void Save()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new BaseLayerJsonConverter());
            options.Converters.Add(new TriggerJsonConverter());
            File.WriteAllText(FilePath, JsonSerializer.Serialize(Current, options));
        }
    }
}
