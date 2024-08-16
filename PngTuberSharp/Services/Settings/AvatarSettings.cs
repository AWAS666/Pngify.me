using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngTuberSharp.Services.Settings
{
    public class AvatarSettings
    {
        public string Open { get; set; } = "Assets/open.png";
        public string Closed { get; set; } = "Assets/closed.png";

        public EventHandler Refresh;

    }
}
