using PngTuberSharp.Services;
using PngTuberSharp.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngTuberSharp.Services.Settings
{
    public class AppSettings
    {
        public MicroPhoneSettings Microphone { get; set; } = new();
        public BackgroundSettings Background { get; set; } = new();
        public AvatarSettings Avatar { get; set; } = new();
        public LayerSettings LayerSetup { get; set; } = new();

    }
}
