using Avalonia.Controls;
using PngTuberSharp.Services.Settings;
using PngTuberSharp.ViewModels.Helper;

namespace PngTuberSharp.Views.Helper;

public partial class TriggerView : UserControl
{
    public TriggerView()
    {
        InitializeComponent();
        DataContext = new TriggerViewModel(new HotkeyTrigger());
    }
}