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

namespace PngifyMe.Services.TTSPet
{
    public class LLMProvider
    {
        public OpenAIService LLMService { get; private set; }

        public LLMProvider()
        {
            Init();
        }

        public void Init()
        {
            var options = new OpenAiOptions()
            {
                ApiKey = SettingsManager.Current.LLM.OpenAIKey
            };
            if (!string.IsNullOrEmpty(SettingsManager.Current.LLM.Domain))
                options.BaseDomain = SettingsManager.Current.LLM.Domain;
            LLMService = new OpenAIService(options, new HttpClient() { Timeout = TimeSpan.FromSeconds(3) });
        }

        public async Task<string> GetResponse(string input)
        {
            try
            {
                var completionResult = await LLMService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
                {
                    Messages = new List<ChatMessage>
                {
                    ChatMessage.FromSystem($@"{SettingsManager.Current.LLM.SystemPrompt}
### Additional Info:
Current Date and time: {DateTime.Now}"),
                    ChatMessage.FromUser(input)
                },
                    Model = SettingsManager.Current.LLM.ModelName,
                });
                return completionResult.Choices.First().Message.Content;
            }
            catch (Exception e)
            {
                Log.Error($"Openai error: " + e.Message, e);
                return "Oh, oh I ran into an issue";
            }
        }

        public async Task<Stream?> GenerateSpeech(string input)
        {
            var speech = await LLMService.CreateSpeech<Stream>(new AudioCreateSpeechRequest()
            {
                Input = input,
                Model = "tts-1",
                Voice = SettingsManager.Current.LLM.TTSVoice.ToString().ToLower(),
            });

            return speech.Data;
        }
    }
}
