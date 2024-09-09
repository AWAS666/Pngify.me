using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Services.TTSPet.Settings;
using PngifyMe.Services.TTSPet.StreamElements;

namespace PngifyMe.Services.Settings
{
    public partial class LLMSettings : ObservableObject
    {      
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
        private string tTSSystem = "StreamElements";


        // storing each of these seperatly, as these might have user api keys in them
        // throwing away on switch might be a hbad idea...
        public OpenAITTSSettings OpenAITTS { get; set; } = new();
        public StreamElementsTTSSettings StreamElementsTTS { get; set; } = new();
    }
}
