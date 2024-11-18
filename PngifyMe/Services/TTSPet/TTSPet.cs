using PngifyMe.Services.Settings;
using PngifyMe.Services.TTSPet.OpenAI;
using PngifyMe.Services.TTSPet.StreamElements;
using PngifyMe.Services.TTSPet.TikTok;
using PngifyMe.Services.Twitch;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;
using static System.Net.Mime.MediaTypeNames;

namespace PngifyMe.Services.TTSPet
{
    public static class TTSPet
    {
        private static Task task;
        private static LLMSettings settings;

        public static OpenAILLM LLMProvider { get; set; } = new();
        public static ITTSProvider TTSProvider { get; set; }

        public static List<LLMMessage> Queue { get; set; } = new();
        public static List<LLMMessage> History { get; set; } = new();

        public static EventHandler<LLMMessage> NewOrUpdated;

        static TTSPet()
        {
            TwitchEventSocket.BitsUsed += BitsUsed;
            TwitchEventSocket.RedeemFull += RedeemUsed;
            TwitchEventSocket.NewFollower += NewFollower;
            TwitchEventSocket.NewChat += NewChat;
            SetupTTS();
            settings = SettingsManager.Current.LLM;
            task = Task.Run(ProcessQueue);
        }


        public static void Reload()
        {
            LLMProvider = new();
            SetupTTS();
        }


        public static void SetupTTS()
        {
            switch (SettingsManager.Current.LLM.TTSSystem)
            {
                case "StreamElements":
                    TTSProvider = new StreamElementsTTS();
                    break;
                case "OpenAI":
                    TTSProvider = new OpenAITTS();
                    break;
                case "TikTok":
                    TTSProvider = new TikTokTTS();
                    break;
                default:
                    break;
            }
        }

        private static void NewFollower(object? sender, string e)
        {
            var msg = new LLMMessage()
            {
                Input = $"Thanks {e} for following the channel",
                ReadInput = true,
            };
            QueueMsg(msg);
        }

        public static void QueueMsg(LLMMessage msg)
        {
            if (SettingsManager.Current.Profile.Active.Type != ProfileType.TTS)
                return;
            Queue.Add(msg);
            NewOrUpdated?.Invoke(null, msg);
        }

        private static void RedeemUsed(object? sender, ChannelPointsCustomRewardRedemption e)
        {
            if (!e.Reward.Title.Equals(SettingsManager.Current.LLM.Redeem, StringComparison.CurrentCultureIgnoreCase))
                return;
            if (settings.JustRead) return;
            var msg = new LLMMessage()
            {
                Input = string.IsNullOrEmpty(e.UserInput) ? $"{e.UserName} used the redeem: {e.Reward.Title}" : e.UserInput,
                UserName = e.UserName,
            };
            QueueMsg(msg);
        }

        private static void NewChat(object? sender, ChannelChatMessage e)
        {
            string message = e.Message.Text;
            // trigger on everything
            if (settings.ChatTriggerEverything)
            {
                var msg1 = new LLMMessage()
                {
                    Input = message,
                    UserName = e.ChatterUserName,
                };
                QueueMsg(msg1);
                return;
            }
            if (string.IsNullOrEmpty(settings.ChatTrigger) ||
                !message.StartsWith(settings.ChatTrigger))
                return;

            message = message.Replace(settings.ChatTrigger, string.Empty).Trim();

            var msg = new LLMMessage()
            {
                Input = message,
                UserName = e.ChatterUserName,
            };
            QueueMsg(msg);
        }

        private static void BitsUsed(object? sender, ChannelCheer e)
        {
            if (e.Bits < settings.MinBits) return;

            LLMMessage msg;
            if (settings.JustRead)
                msg = new LLMMessage()
                {
                    Input = $"{e.UserName} donated {e.Bits} with the message: {e.Message}",
                    ReadInput = true,
                    UserName = e.UserName,
                };
            else
                msg = new LLMMessage()
                {
                    Input = e.Message,
                    UserName = e.UserName,
                };
            QueueMsg(msg);
        }

        static async Task ProcessQueue()
        {
            while (true)
            {
                LLMMessage? item = null;
                try
                {
                    item = Queue.FirstOrDefault(x => !x.Read && x.Retries < 3);
                    if (item == null)
                    {
                        continue;
                    }

                    if (!item.ReadInput)
                        await GetResponse(item);

                    await ReadText(ReplaceWithSafe(item.ToRead));

                    item.Read = true;
                }
                catch (Exception e)
                {
                    Log.Error("Error in LLM loop: " + e.Message, e);
                    await Task.Delay(1000);
                    if (item != null)
                        item.Retries += 1;
                }
                finally
                {
                    await Task.Delay(500);
                }
            }
        }

        private static async Task GetResponse(LLMMessage item)
        {
            if (!string.IsNullOrEmpty(item.Output)) return;
            item.Output = await LLMProvider.GetResponse(item.Input, item.UserName, History);

            History.Add(item);

            while (History.Count > 4)
            {
                History.RemoveAt(0);
            }
        }

        public static async Task ReadText(string input)
        {
            foreach (var item in Regex.Split(input, @"(?<=[.!?])\s+"))
            {
                try
                {
                    var audio = await TTSProvider.GenerateSpeech(item);
                    await AudioService.PlaySound(audio);
                }
                catch (Exception e)
                {
                    Log.Error($"Skipped text: {item} because of error {e.Message}");
                }
            }
        }

        public static void QueueText(string text, bool readOnly = false)
        {
            var msg = new LLMMessage()
            {
                Input = text,
            };
            QueueMsg(msg);
        }

        public static bool CheckTextSafe(string text)
        {
            foreach (var keyword in settings.BannedPhrases)
            {
                if (Regex.Match(text, @$"\b{keyword}\b", RegexOptions.IgnoreCase).Success)
                {
                    return false;
                }
            }
            return true;
        }

        public static string ReplaceWithSafe(string text)
        {
            foreach (var keyword in settings.BannedPhrases)
            {
                if (Regex.Match(text, @$"\b{keyword}\b", RegexOptions.IgnoreCase).Success)
                {
                    text = Regex.Replace(text, @$"\b{keyword}\b", settings.Replacement, RegexOptions.IgnoreCase);
                }
            }
            return text;
        }
    }
}
