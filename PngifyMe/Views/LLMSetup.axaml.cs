using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using PngifyMe.Services;
using PngifyMe.Services.Settings;
using PngifyMe.ViewModels;
using System.Linq;

namespace PngifyMe.Views;

public partial class LLMSetup : UserControl
{
    public LLMSetup()
    {
        InitializeComponent();
        DataContext = SettingsManager.Current.LLM;
    }    
}