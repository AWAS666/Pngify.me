using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            {
                Log.Error("Missing openai key");
                return;
            }

            var options = new OpenAiOptions()
            {
                ApiKey = SettingsManager.Current.LLM.OpenAIKey
            };
            if (!string.IsNullOrEmpty(SettingsManager.Current.LLM.Domain))
                options.BaseDomain = SettingsManager.Current.LLM.Domain;
            LLMService = new OpenAIService(options, new HttpClient() { Timeout = TimeSpan.FromSeconds(6) });
        }

        public async Task<string> GetResponse(string input, string userName, List<LLMMessage> history)
        {
            try
            {
                if (LLMService == null)
                    Init();
                string? user = userName != null ? Regex.Replace(userName, @"[^a-zA-Z\d_-]", "") : null;
                var messages = new List<ChatMessage>
                {
                    ChatMessage.FromSystem($@"{SettingsManager.Current.LLM.SystemPrompt}
### Additional Info:
Current Date and time: {DateTime.Now}"),

                };
                // add history
                history.ForEach(x => messages.AddRange(x.ToChatMessage()));
                messages.Add(ChatMessage.FromUser(input, user));

                var completionResult = await LLMService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
                {
                    Messages = messages,
                    Model = SettingsManager.Current.LLM.ModelName,
                    FrequencyPenalty = 1f,
                    PresencePenalty = 1f,
                    MaxTokens = SettingsManager.Current.LLM.MaxTokens,
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
