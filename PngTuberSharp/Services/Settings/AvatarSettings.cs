using System;

namespace PngTuberSharp.Services.Settings
{
    public class AvatarSettings
    {
        public string Open { get; set; } = "Assets/open.png";
        public string Closed { get; set; } = "Assets/closed.png";

        public EventHandler Refresh;

    }
}
