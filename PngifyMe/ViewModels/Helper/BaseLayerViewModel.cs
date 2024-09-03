﻿using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Layers;
using PngifyMe.Layers.Helper;
using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.Json.Serialization;

namespace PngifyMe.ViewModels.Helper
{
    public partial class BaseLayerViewModel : ObservableObject
    {
        public BaseLayer LayerModel { get; }

        public BaseLayerViewModel(BaseLayer layer)
        {
            LayerModel = layer;
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
            foreach (var prop in LayerModel.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                // Ignore JSON ignored properties
                if (Attribute.IsDefined(prop, typeof(JsonIgnoreAttribute)))
                    continue;

                var propertyViewModel = new PropertyViewModel
                {
                    Name = prop.Name,
                    Value = prop.GetValue(LayerModel)?.ToString()
                };

                var unitAttribute = prop.GetCustomAttribute<UnitAttribute>();
                if (unitAttribute != null)
                {
                    propertyViewModel.Unit = unitAttribute.Unit;
                }

                PropertyList.Add(propertyViewModel);
            }
        }

        public void Save()
        {
            foreach (var propertyViewModel in PropertyList)
            {
                var prop = LayerModel.GetType().GetProperty(propertyViewModel.Name, BindingFlags.Public | BindingFlags.Instance);

                if (prop != null)
                {
                    var convertedValue = Convert.ChangeType(propertyViewModel.Value, prop.PropertyType);
                    prop.SetValue(LayerModel, convertedValue);
                }
            }
        }
    }
}