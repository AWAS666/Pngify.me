using Avalonia.Controls;
using PngifyMe.ViewModels;

namespace PngifyMe.Views.Helper;

public partial class LayerSelectorView : UserControl
{
    public LayerSelectorView()
    {
        InitializeComponent();
    }

    private void SearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var vm = (LayerSelectorViewModel)DataContext;
        vm.ChangeFilter(((TextBox)e.Source).Text);
    }
}