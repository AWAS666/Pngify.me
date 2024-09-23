using Avalonia;
using Avalonia.Controls;
using PngifyMe.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
            var path = Path.Combine(Specsmanager.BasePath, "screen.json");
            File.WriteAllText(Path.Combine(Specsmanager.BasePath, "screen.json"), JsonSerializer.Serialize(pos));
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
