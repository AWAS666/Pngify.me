using CommunityToolkit.Mvvm.ComponentModel;
using PngTuberSharp.Layers;
using System.Reflection;
using System;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using PngTuberSharp.Layers.Helper;

namespace PngTuberSharp.ViewModels.Helper
{
    public partial class BaseLayerViewModel : ObservableObject
    {
        private BaseLayer _layer;

        public BaseLayerViewModel(BaseLayer layer)
        {
            _layer = layer;
            Name = layer.GetType().Name;
            UpdatePropertyList();
        }

        [ObservableProperty]
        private ObservableCollection<PropertyViewModel> _propertyList = new ObservableCollection<PropertyViewModel>();
        [ObservableProperty]
        private string name;

        private void UpdatePropertyList()
        {
            PropertyList.Clear();
            // Use reflection to get properties
            foreach (var prop in _layer.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                // Ignore JSON ignored properties
                if (Attribute.IsDefined(prop, typeof(JsonIgnoreAttribute)))
                    continue;

                var propertyViewModel = new PropertyViewModel
                {
                    Name = prop.Name,
                    Value = prop.GetValue(_layer)?.ToString()
                };

                var unitAttribute = prop.GetCustomAttribute<UnitAttribute>();
                if (unitAttribute != null)
                {
                    propertyViewModel.Unit = unitAttribute.Unit;
                }

                PropertyList.Add(propertyViewModel);
            }
        }
    }
}
