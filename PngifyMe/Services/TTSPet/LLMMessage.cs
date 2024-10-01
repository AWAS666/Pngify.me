using CommunityToolkit.Mvvm.ComponentModel;
using System;

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
    }
}
