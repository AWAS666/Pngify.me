using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngTuberSharp.Services
{
    public static class TwitchHandler
    {
        public static EventHandler<string> RedeemUsed;
        public static EventHandler<string> BitsUsed;
        public static EventHandler<string> NewFollower;
        public static EventHandler<string> NewSubscriber;
    }
}
