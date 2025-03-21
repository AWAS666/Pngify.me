using PngifyMe.Helpers;
using PngifyMe.Layers.Helper;
using PngifyMe.Services.Settings;
using Serilog;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

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
            //Load();
        }

        public static async Task LoadAsync()
        {
            if (File.Exists(FilePath))
            {
                try
                {
                    using var stream = File.OpenRead(FilePath);
                    Current = await JsonSerializer.DeserializeAsync<AppSettings>(stream, JsonSerializeHelper.GetDefault());
                    // Current = JsonSerializer.Deserialize<AppSettings>(await File.ReadAllTextAsync(FilePath), JsonSerializeHelper.GetDefault());
                }
                catch (Exception e)
                {
                    Current = new();
                    await SaveAsync();
                    Log.Error(e.Message);
                }
            }
            else
            {
                Current = new();
                await SaveAsync();
            }
            Current.Profile.Load();
        }

        public static void Save()
        {
            File.WriteAllText(FilePath, JsonSerializer.Serialize(Current, JsonSerializeHelper.GetDefault()));
        }

        public static async Task SaveAsync()
        {
            await File.WriteAllTextAsync(FilePath, JsonSerializer.Serialize(Current, JsonSerializeHelper.GetDefault()));
        }
    }
}
