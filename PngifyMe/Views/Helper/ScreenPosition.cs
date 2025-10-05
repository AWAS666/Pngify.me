using Avalonia.Controls;
using PngifyMe.Services;
using Serilog;
using System;
using System.IO;
using System.Text.Json;

namespace PngifyMe.Views.Helper
{
    public class ScreenPosition
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }
        public WindowState WindowState { get; set; }

        public static void Save(ScreenPosition pos)
        {
            try
            {
                var path = Path.Combine(Specsmanager.BasePath, "screen.json");
                File.WriteAllText(Path.Combine(Specsmanager.BasePath, "screen.json"), JsonSerializer.Serialize(pos));
            }
            catch (Exception e)
            {
                Log.Error(e, $"Error in saving screen position: {e.Message} | Position: {pos.Width}, {pos.Height}, {pos.Left}, {pos.Top}, {pos.WindowState}");
            }
        }

        public static ScreenPosition Load()
        {
            var path = Path.Combine(Specsmanager.BasePath, "screen.json");
            if (!File.Exists(path))
                return null;
            return JsonSerializer.Deserialize<ScreenPosition>(File.ReadAllText(path));
        }
    }
}
