using Avalonia.Controls;
using PngTuberSharp.ViewModels;

namespace PngTuberSharp.Views;

public partial class AvatarView : UserControl
{
    public AvatarView()
    {
        InitializeComponent();
        DataContext = new AvatarViewModel();
    }
}