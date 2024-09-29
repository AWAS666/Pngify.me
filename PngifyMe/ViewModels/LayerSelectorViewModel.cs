using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irihi.Avalonia.Shared.Contracts;
using PngifyMe.ViewModels.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngifyMe.ViewModels
{
    public partial class LayerSelectorViewModel : ObservableObject, IDialogContext
    {
        public LayersettingViewModel Parent { get; }

        [ObservableProperty]
        private ObservableCollection<Type> allLayers;
        public Type Selected { get; set; }

        public LayerSelectorViewModel() : this(new LayersettingViewModel())
        {
        }

        public LayerSelectorViewModel(LayersettingViewModel parent)
        {
            Parent = parent;
            AllLayers = parent.AllLayers;
        }

        public event EventHandler<object?>? RequestClose;

        [RelayCommand]
        public void Submit()
        {
            Parent.AddNewLayer(Selected);
            Close();
        }

        [RelayCommand]
        public void Cancel()
        {
            Close();
        }

        internal void ChangeFilter(string v)
        {
            AllLayers = new ObservableCollection<Type>(Parent.AllLayers.Where(x => x.Name.Contains(v, StringComparison.CurrentCultureIgnoreCase)));
        }

        public void Close()
        {
            RequestClose?.Invoke(this, null);
        }
    }
}
