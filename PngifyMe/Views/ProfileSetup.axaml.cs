using Avalonia.Controls;
using Avalonia.Data;
using PngifyMe.Services;
using PngifyMe.ViewModels;

namespace PngifyMe.Views;

public partial class ProfileSetup : UserControl
{
    public ProfileSetup()
    {
        InitializeComponent();
        DataContext = new ProfileSettViewModel();
    }
}