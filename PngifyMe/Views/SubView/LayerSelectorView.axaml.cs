using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PngifyMe.Services.Settings;
using PngifyMe.ViewModels;
using PngifyMe.ViewModels.Helper;
using System.Linq;
using System.Net;

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