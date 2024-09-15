using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Services;
using PngifyMe.Services.Settings;
using PngifyMe.Services.TTSPet;
using PngifyMe.Services.TTSPet.OpenAI;
using PngifyMe.Services.TTSPet.StreamElements;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngifyMe.ViewModels
{
    public partial class LLMProviderViewModel : ObservableObject
    {
        [ObservableProperty]
        private LLMSettings settings;

        [ObservableProperty]
        private ObservableCollection<LLMMessage> messages;

        public Dictionary<string, ITTSSettings> TTSProviders { get; }
        public IEnumerable<string> TTSProviderKeys => TTSProviders.Keys;

        [ObservableProperty]
        private ITTSSettings tTSSettings;

        public LLMProviderViewModel()
        {
            Settings = SettingsManager.Current.LLM;
            Messages = new ObservableCollection<LLMMessage>(TTSPet.Queue);
            TTSPet.NewOrUpdated += UpdateMessages;

            TTSProviders = new Dictionary<string, ITTSSettings>()
            {
                {"StreamElements", Settings.StreamElementsTTS },
                {"OpenAI", Settings.OpenAITTS },
                {"TikTok", Settings.TikTokSettings },
            };
            SetTTS();
        }

        public void SetTTS()
        {
            TTSSettings = TTSProviders[Settings.TTSSystem];
        }

        private void UpdateMessages(object? sender, LLMMessage e)
        {
            if (!Messages.Contains(e))
                Messages.Add(e);
        }
    }
}
