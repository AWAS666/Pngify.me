using NAudio.Wave;
using PngifyMe.Services.TTSPet.OpenAI;
using PngifyMe.Services.TTSPet.StreamElements;
using PngifyMe.Services.Twitch;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;

namespace PngifyMe.Services.TTSPet
{
    public static class TTSPet
    {
        private static Task task;

        public static OpenAILLM LLMProvider { get; set; } = new();
        public static ITTSProvider TTSProvider { get; set; }

        public static List<LLMMessage> Queue { get; set; } = new();

        public static EventHandler<LLMMessage> NewOrUpdated;

        static TTSPet()
        {
            TwitchEventSocket.BitsUsed += BitsUsed;
            TwitchEventSocket.RedeemFull += RedeemUsed;
            TwitchEventSocket.NewFollower += NewFollower;
            SetupTTS();
            task = Task.Run(ProcessQueue);
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

        private static void QueueMsg(LLMMessage msg)
        {
            if (!SettingsManager.Current.LLM.Enabled)
                return;
            Queue.Add(msg);
            NewOrUpdated?.Invoke(null, msg);
        }

        private static void RedeemUsed(object? sender, ChannelPointsCustomRewardRedemption e)
        {
            if (e.Reward.Title.ToLower() != SettingsManager.Current.LLM.Redeem.ToLower())
                return;
            var msg = new LLMMessage()
            {
                Input = string.IsNullOrEmpty(e.UserInput) ? $"{e.UserName} used the redeem: {e.Reward.Title}" : e.UserInput,
                UserName = e.UserName,
            };
            QueueMsg(msg);
        }

        private static void BitsUsed(object? sender, ChannelCheer e)
        {
            var msg = new LLMMessage()
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
                try
                {
                    var item = Queue.FirstOrDefault(x => !x.Read);
                    if (item == null)
                    {
                        continue;
                    }

                    if (!item.ReadInput)
                        await GetResponse(item);

                    await ReadText(item.ToRead);

                    item.Read = true;
                }
                catch (Exception e)
                {
                    Log.Error("Error in LLM loop: " + e.Message, e);
                    await Task.Delay(1000);
                }
                finally
                {
                    await Task.Delay(500);
                }
            }
        }

        private static async Task GetResponse(LLMMessage item)
        {
            item.Output = await LLMProvider.GetResponse(item.Input, item.UserName);
        }       

        public static async Task ReadText(string input)
        {
            var audio = await TTSProvider.GenerateSpeech(input);
            WaveStream reader = new Mp3FileReader(audio);
            var player = new WaveOutEvent();

            player.Init(reader);

            // Subscribe to the PlaybackStopped event to dispose of resources when playback is done
            player.PlaybackStopped += (sender, args) =>
            {
                player.Dispose();
                reader.Dispose();
                audio.Dispose();
            };
            await Task.Run(async () =>
            {
                //await Task.Delay(Random.Shared.Next(1, 200));
                player.Play();
                while (player.PlaybackState != PlaybackState.Stopped)
                {
                    await Task.Delay(50);
                }
            });
        }

        public static void QueueText(string text)
        {
            var msg = new LLMMessage()
            {
                Input = text,
            };
            QueueMsg(msg);
        }
    }
}
