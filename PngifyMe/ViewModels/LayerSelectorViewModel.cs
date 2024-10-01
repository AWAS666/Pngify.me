using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irihi.Avalonia.Shared.Contracts;
using PngifyMe.Layers.Helper;
using PngifyMe.ViewModels.Helper;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace PngifyMe.ViewModels
{
    public partial class LayerSelectorViewModel : ObservableObject, IDialogContext
    {
        public LayersettingViewModel Parent { get; }

        [ObservableProperty]
        private ObservableCollection<LayerClass> allLayers;

        [ObservableProperty]
        private ObservableCollection<LayerClass> viewLayers;
        public LayerClass Selected { get; set; }

        public LayerSelectorViewModel() : this(new LayersettingViewModel())
        {
        }

        public LayerSelectorViewModel(LayersettingViewModel parent)
        {
            Parent = parent;
            AllLayers = new ObservableCollection<LayerClass>(parent.AllLayers.Select(LayerClass.CreateFromBaseLayer));
            ViewLayers = AllLayers;
        }

        public event EventHandler<object?>? RequestClose;

        [RelayCommand]
        public void Submit()
        {
            if (Selected != null)
                Parent.AddNewLayer(Selected.Type);
            Close();
        }

        [RelayCommand]
        public void Cancel()
        {
            Close();
        }

        internal void ChangeFilter(string v)
        {
            ViewLayers = new ObservableCollection<LayerClass>(AllLayers.Where(x => x.Name.Contains(v, StringComparison.CurrentCultureIgnoreCase)));
        }

        public void Close()
        {
            RequestClose?.Invoke(this, null);
        }
    }

    public class LayerClass
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Type Type { get; set; }

        public static LayerClass CreateFromBaseLayer(Type Type)
        {
            var attribute = (LayerDescriptionAttribute)Attribute.GetCustomAttribute(Type, typeof(LayerDescriptionAttribute));
            return new LayerClass()
            {
                Name = Type.Name,
                Type = Type,
                Description = attribute?.Description ?? "??"
            };
        }
    }
}
