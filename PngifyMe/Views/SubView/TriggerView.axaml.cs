using Avalonia.Controls;
using PngifyMe.Services.Settings;
using PngifyMe.ViewModels.Helper;

namespace PngifyMe.Views.Helper;

public partial class TriggerView : UserControl
{
    public TriggerView()
    {
        InitializeComponent();
        DataContext = new TriggerViewModel(new HotkeyTrigger());
    }
}