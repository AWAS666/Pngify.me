using CommunityToolkit.Mvvm.ComponentModel;
using OpenAI.ObjectModels.RequestModels;
using System;
using System.Collections.Generic;

namespace PngifyMe.Services.TTSPet
{
    public partial class LLMMessage : ObservableObject
    {
        [ObservableProperty]
        private string input;

        [ObservableProperty]
        private string output;

        [ObservableProperty]
        private string? userName = null;

        [ObservableProperty]
        private bool read;

        [ObservableProperty]
        private int retries;

        public bool ReadInput { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;

        public string ToRead => ReadInput ? Input : Output;

        public IEnumerable<ChatMessage> ToChatMessage()
        {
            return new List<ChatMessage>()
            {
                ChatMessage.FromUser(Input,UserName),
                ChatMessage.FromAssistant(Output),
            };
        }
    }
}
