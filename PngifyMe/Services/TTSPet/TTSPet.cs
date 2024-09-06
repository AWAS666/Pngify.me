using NAudio.Wave;
using PngifyMe.Services.Twitch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;

namespace PngifyMe.Services.TTSPet
{
    public static class TTSPet
    {
        public static LLMProvider LLMProvider { get; set; } = new();

        static TTSPet()
        {
            TwitchEventSocket.BitsUsed += BitsUsed;
            TwitchEventSocket.RedeemFull += RedeemUsed;
            TwitchEventSocket.NewFollower += NewFollower;
        }

        private static void NewFollower(object? sender, string e)
        {

        }

        private static void RedeemUsed(object? sender, ChannelPointsCustomRewardRedemption e)
        {
        }

        private static void BitsUsed(object? sender, ChannelCheer e)
        {

        }

        public static async Task<System.IO.Stream?> GenerateResponse(string input)
        {
            var output = await LLMProvider.GetResponse(input);
            return await LLMProvider.GenerateSpeech(output);
        }

        public static async Task GenerateAndRead(string input)
        {
            var audio = await GenerateResponse(input);
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
    }
}
