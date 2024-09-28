using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace PngifyMe.ViewModels.Helper
{
    public partial class PropertyViewModel : ObservableObject
    {
        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private object value;

        [ObservableProperty]
        private string unit;

        [ObservableProperty]
        private Type type;

        [ObservableProperty]
        private bool filePicker;

        [ObservableProperty]
        private bool folderPicker;

        public bool Picker => FilePicker || FolderPicker;

        public FilePickerFileType? PickFilter { get; internal set; }
    }
}
