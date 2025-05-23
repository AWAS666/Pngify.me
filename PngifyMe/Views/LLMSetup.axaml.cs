using Avalonia.Controls;
using Avalonia.Interactivity;
using PngifyMe.Services.TTSPet;
using PngifyMe.ViewModels;

namespace PngifyMe.Views;

public partial class LLMSetup : UserControl
{
    public LLMSetup()
    {
        InitializeComponent();
        DataContext = new LLMProviderViewModel();
    }

    private void TriggerTest(object sender, RoutedEventArgs e)
    {
        TTSPet.QueueText(inputText.Text);
    }

    private void JustRead(object sender, RoutedEventArgs e)
    {
        // set input and output to same to just read it
        TTSPet.QueueMsg(new LLMMessage()
        {
            Input = inputText.Text,
            Output = inputText.Text
        });
    }

    private void UpdateTTSProvider(object sender, SelectionChangedEventArgs e)
    {
        var vm = (LLMProviderViewModel)DataContext;
        vm.SetTTS();
        TTSPet.SetupTTS();
    }

    private void ApplyLLMSettings(object sender, RoutedEventArgs e)
    {
        TTSPet.Reload();
    }

    private void ClearContext(object sender, RoutedEventArgs e)
    {
        var vm = (LLMProviderViewModel)DataContext;
        vm.Messages.Clear();
        TTSPet.ClearContext();
    }
}