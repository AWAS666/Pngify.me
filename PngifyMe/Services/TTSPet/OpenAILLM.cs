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
using System.Text.RegularExpressions;

namespace PngifyMe.Services.TTSPet
{
    public class OpenAILLM
    {
        public OpenAIService LLMService { get; private set; }

        public OpenAILLM()
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

        public async Task<string> GetResponse(string input, string userName)
        {
            try
            {
                string? user = userName != null ? Regex.Replace(userName, @"[^a-zA-Z\d_-]", "") : null;
                var completionResult = await LLMService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
                {
                    Messages = new List<ChatMessage>
                {
                    ChatMessage.FromSystem($@"{SettingsManager.Current.LLM.SystemPrompt}
### Additional Info:
Current Date and time: {DateTime.Now}"),
                    ChatMessage.FromUser(input, user)
                },
                    Model = SettingsManager.Current.LLM.ModelName,
                    FrequencyPenalty = 1f,
                    PresencePenalty = 1f,
                    MaxTokens = 256,
                    Temperature = 0.8f
                });
                return completionResult.Choices.First().Message.Content;
            }
            catch (Exception e)
            {
                Log.Error($"Openai error: " + e.Message, e);
                return "Oh, oh I ran into an issue";
            }
        }
    }
}
