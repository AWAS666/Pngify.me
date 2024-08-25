using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngTuberSharp.Services.Settings
{
    public class TitsSettings
    {
        public bool Enabled { get; set; } = false;
        public bool HitLinesVisible { get; set; } = false;

        public bool ShowHitlines => Enabled && HitLinesVisible;
    }
}
