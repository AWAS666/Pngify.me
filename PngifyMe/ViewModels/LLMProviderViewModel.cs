using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Services;
using PngifyMe.Services.Settings;
using PngifyMe.Services.TTSPet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngifyMe.ViewModels
{
    public partial class LLMProviderViewModel : ObservableObject
    {
        [ObservableProperty]
        private LLMSettings settings;
        public static List<OpenAITTSVoices> TTSTypes { get; set; } = new List<OpenAITTSVoices>(Enum.GetValues(typeof(OpenAITTSVoices)).Cast<OpenAITTSVoices>());
        public LLMProviderViewModel()
        {
            Settings = SettingsManager.Current.LLM;
        }
    }
}
