using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PngifyMe.Services.TTSPet.Settings
{
    public partial class OpenAITTSSettings : ObservableObject, ITTSSettings
    {
        [ObservableProperty]
        private string tTSModel = "tts-1";

        [ObservableProperty]
        private OpenAITTSVoices tTSVoice = OpenAITTSVoices.Echo;

        public static List<OpenAITTSVoices> TTSTypes { get; set; } = new List<OpenAITTSVoices>(Enum.GetValues(typeof(OpenAITTSVoices)).Cast<OpenAITTSVoices>());

    }

    public enum OpenAITTSVoices
    {
        Alloy,
        Echo,
        Fable,
        Onyx,
        Nova,
        Shimmer
    }
}
