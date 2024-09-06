using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace PngifyMe.ViewModels.Helper
{
    public partial class PropertyViewModel : ObservableObject
    {
        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string value;

        [ObservableProperty]
        private string unit;

        [ObservableProperty]
        private Type type;

        [ObservableProperty]
        private bool filePicker;      
    }
}
