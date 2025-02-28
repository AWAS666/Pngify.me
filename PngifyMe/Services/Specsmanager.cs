using CommunityToolkit.Mvvm.ComponentModel;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace PngifyMe.Services
{
    public static class Specsmanager
    {
        public static int Width { get; }
        public static int Height { get; }
        public static float ScaleFactor { get; } = 0.9f;
        public static int FPS { get; }

        public static SpecsSettings Settings { get; private set; }
        public static string BasePath { get; }
        public static string FilePath { get; }
        public static int TitsSize { get; set; }

        static Specsmanager()
        {

            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder) + "\\PngifyMe";
            Directory.CreateDirectory(path);
            BasePath = path;
            FilePath = Path.Combine(path, "specs.json");
            Load();

            switch (Settings.Mode)
            {
                case SpecMode.VeryLow:
                    Width = 960;
                    Height = 540;
                    FPS = 60;
                    TitsSize = 30;
                    break;
                case SpecMode.Low:
                    Width = 1280;
                    Height = 720;
                    FPS = 60;
                    TitsSize = 50;
                    break;
                case SpecMode.Normal:
                    Width = 1920;
                    Height = 1080;
                    FPS = 60;
                    TitsSize = 75;
                    break;
                case SpecMode.Ultra:
                    Width = 3840;
                    Height = 2160;
                    FPS = 60;
                    TitsSize = 100;
                    break;
                default:
                    break;
            }
        }

        public static void Load()
        {
            if (File.Exists(FilePath))
            {
                try
                {
                    Settings = JsonSerializer.Deserialize<SpecsSettings>(File.ReadAllText(FilePath));
                }
                catch (Exception e)
                {
                    Settings = new();
                    Save();
                    Log.Error(e.Message);
                }
            }
            else
            {
                Settings = new();
                Save();
            }
        }

        public static void Save()
        {
            File.WriteAllText(FilePath, JsonSerializer.Serialize(Settings));
        }
    }

    public partial class SpecsSettings : ObservableObject
    {
        [ObservableProperty]
        private SpecMode mode = SpecMode.Normal;

        public static List<SpecMode> Modes { get; set; } = new List<SpecMode>(Enum.GetValues(typeof(SpecMode)).Cast<SpecMode>());

    }

    public enum SpecMode
    {
        VeryLow,
        Low,
        Normal,
        Ultra
    }
}
