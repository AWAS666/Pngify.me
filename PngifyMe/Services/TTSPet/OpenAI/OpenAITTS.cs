using OpenAI.Managers;
using OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using OpenAI.Interfaces;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels;
using Serilog;
using System.IO;

namespace PngifyMe.Services.TTSPet.OpenAI
{
    public class OpenAITTS : ITTSProvider
    {
        public OpenAIService LLMService { get; private set; }

        public OpenAITTS()
        {
            Init();
        }

        public void Init()
        {
            if (string.IsNullOrEmpty(SettingsManager.Current.LLM.OpenAIKey))
                return;
            var options = new OpenAiOptions()
            {
                ApiKey = SettingsManager.Current.LLM.OpenAIKey
            };
            if (!string.IsNullOrEmpty(SettingsManager.Current.LLM.Domain))
                options.BaseDomain = SettingsManager.Current.LLM.Domain;
            LLMService = new OpenAIService(options, new HttpClient() { Timeout = TimeSpan.FromSeconds(3) });
        }

        public async Task<Stream?> GenerateSpeech(string input)
        {
            if(LLMService == null)
                Init();
            var speech = await LLMService.CreateSpeech<Stream>(new AudioCreateSpeechRequest()
            {
                Input = input,
                Model = SettingsManager.Current.LLM.OpenAITTS.TTSModel,
                Voice = SettingsManager.Current.LLM.OpenAITTS.TTSVoice.ToString().ToLower(),
            });

            return speech.Data;
        }
    }
}
