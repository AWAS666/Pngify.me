﻿using PngTuberSharp.Layers;
using PngTuberSharp.Services.Settings;
using System;
using System.IO;
using System.Text.Json;

namespace PngTuberSharp.Services
{
    public static class SettingsManager
    {
        public static string FilePath { get; }

        public static AppSettings Current { get; private set; }

        static SettingsManager()
        {
            // load settings
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder) + "\\PngTuberSharp";
            Directory.CreateDirectory(path);
            FilePath = Path.Join(path, "settings.json");
            Load();
        }

        public static void Load()
        {
            if (File.Exists(FilePath))
            {
                var options = new JsonSerializerOptions();
                options.Converters.Add(new BaseLayerJsonConverter());
                options.Converters.Add(new TriggerJsonConverter());
                Current = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(FilePath), options);
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
