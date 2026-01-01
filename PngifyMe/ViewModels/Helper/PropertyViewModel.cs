using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;

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

        [ObservableProperty]
        private bool isEnum;

        [ObservableProperty]
        private Type enumType;

        [ObservableProperty]
        private ObservableCollection<string> enumValues;

        public bool Picker => FilePicker || FolderPicker;

        public FilePickerFileType? PickFilter { get; internal set; }
    }
}
