﻿using PngifyMe.Services.TTSPet.Settings;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace PngifyMe.Services.TTSPet.StreamElements
{
    public class StreamElementsTTS : ITTSProvider
    {
        private HttpClient client;
        private StreamElementsTTSSettings settings;

        public StreamElementsTTS()
        {
            var clientHandler = new HttpClientHandler();
            client = new HttpClient(clientHandler);
            client.Timeout = TimeSpan.FromSeconds(10);
            settings = SettingsManager.Current.LLM.StreamElementsTTS;
        }

        public async Task<Stream?> GenerateSpeech(string input)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://api.streamelements.com/kappa/v2/speech?voice={settings.Voice.Voice}&text={input}"),
            };
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync();
        }
    }
}
