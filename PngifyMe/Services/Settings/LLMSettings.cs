using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Services.TTSPet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngifyMe.Services.Settings
{
    public partial class LLMSettings : ObservableObject
    {
        [ObservableProperty]
        private bool enabled;

        [ObservableProperty]
        private uint minBits;

        [ObservableProperty]
        private string redeem;

        [ObservableProperty]
        private bool reactFollowers;

        /// <summary>
        /// this should allow any openai compatible endpoint
        /// </summary>
        [ObservableProperty]
        private string domain;
        
        /// <summary>
        /// todo: move this somewhere else??
        /// </summary>
        [ObservableProperty]
        private string openAIKey;
        
        [ObservableProperty]
        private string systemPrompt = "You are a streamers TTSPet reading their donations whilst trying to be as sassy about it as possible.";
        
        [ObservableProperty]
        private string modelName = "gpt-4o-mini";

        [ObservableProperty]
        private string tTSModel = "tts-1";

        [ObservableProperty]
        private OpenAITTS tTSVoice = OpenAITTS.Echo;

        public static List<OpenAITTS> TTSTypes { get; set; } = new List<OpenAITTS>(Enum.GetValues(typeof(OpenAITTS)).Cast<OpenAITTS>());
    }
}
