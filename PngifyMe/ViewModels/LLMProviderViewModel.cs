using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Services;
using PngifyMe.Services.Settings;
using PngifyMe.Services.TTSPet;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
            Dispatcher.UIThread.Invoke(() =>
            {
                if (!Messages.Contains(e))
                    Messages.Add(e);
            });            
        }
    }
}
