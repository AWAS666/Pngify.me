using CommunityToolkit.Mvvm.ComponentModel;
using CppSharp.Types.Std;
using System.Collections.Generic;

namespace PngifyMe.Services.TTSPet.Settings
{
    public partial class StreamElementsTTSSettings : ObservableObject, ITTSSettings
    {
        [ObservableProperty]
        private string voice = "Brian";

        //https://lazypy.ro/tts/
        public List<string> VoiceList { get; } = new()
        {
            "Brian",
            "Emma"
        };
    }
}
