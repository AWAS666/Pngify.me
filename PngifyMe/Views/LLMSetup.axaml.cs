using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using PngifyMe.Services;
using PngifyMe.Services.Settings;
using PngifyMe.Services.TTSPet;
using PngifyMe.ViewModels;
using System.Linq;
using System.Threading.Tasks;

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
}