using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
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

    private void HotkeyDown(object sender, KeyEventArgs e)
    {
        var vm = DataContext as TriggerViewModel;
        vm.OnKeyDown(e);
    }
}