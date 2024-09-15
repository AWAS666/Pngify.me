using PngifyMe.Services.TTSPet.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PngifyMe.Services.TTSPet.TikTok
{
    public class TikTokTTS : ITTSProvider
    {
        private HttpClient client;
        private TikTokSettings settings;

        /// <summary>
        /// build using this as reference
        /// https://github.com/mark-rez/TikTok-Voice-TTS/tree/main
        /// voices: https://lazypy.ro/tts
        /// </summary>
        public TikTokTTS()
        {
            var clientHandler = new HttpClientHandler();
            client = new HttpClient(clientHandler);
            settings = SettingsManager.Current.LLM.TikTokSettings;
        }

        public async Task<Stream?> GenerateSpeech(string input)
        {
            var entry = new TikTokTTSRequest();
            entry.Text = input;
            entry.Voice = settings.Voice.Code;

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(settings.Endpoint),
                Content = new StringContent(JsonSerializer.Serialize(entry))
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue("application/json")
                    }
                }
            };
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var resp = JsonSerializer.Deserialize<TikTokTTSResponse>(await response.Content.ReadAsStringAsync());
            byte[] byteArray = Convert.FromBase64String(resp.Data);

            // Create a memory stream using the byte array
            MemoryStream stream = new MemoryStream(byteArray);

            return stream;
        }
    }

    public class TikTokTTSRequest
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("voice")]
        public string Voice { get; set; }
    }

    public class TikTokTTSResponse
    {
        [JsonPropertyName("data")]
        public string Data { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }


}
