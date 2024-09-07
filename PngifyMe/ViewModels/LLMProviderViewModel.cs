using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Services;
using PngifyMe.Services.Settings;
using PngifyMe.Services.TTSPet;
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
        public static List<OpenAITTSVoices> TTSTypes { get; set; } = new List<OpenAITTSVoices>(Enum.GetValues(typeof(OpenAITTSVoices)).Cast<OpenAITTSVoices>());
        public LLMProviderViewModel()
        {
            Settings = SettingsManager.Current.LLM;
            Messages = new ObservableCollection<LLMMessage>(TTSPet.Queue);
            TTSPet.NewOrUpdated += UpdateMessages;
        }

        private void UpdateMessages(object? sender, LLMMessage e)
        {
            if (!Messages.Contains(e))
                Messages.Add(e);
        }
    }
}
