using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Layers;
using PngifyMe.Layers.Helper;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.Json.Serialization;

namespace PngifyMe.ViewModels.Helper
{
    public partial class BaseLayerViewModel : ObservableObject
    {
        public BaseLayer LayerModel { get; }
        public LayersettingViewModel Parent { get; }

        public BaseLayerViewModel(BaseLayer layer, LayersettingViewModel parent)
        {
            LayerModel = layer;
            Name = layer.GetType().Name;
            UpdatePropertyList();
            Parent = parent;
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
                    Value = prop.PropertyType == typeof(bool) ? (bool)prop.GetValue(LayerModel) : prop.GetValue(LayerModel)?.ToString(),
                    Type = prop.PropertyType,
                };

                var unitAttribute = prop.GetCustomAttribute<UnitAttribute>();
                if (unitAttribute != null)
                {
                    propertyViewModel.Unit = unitAttribute.Unit;
                }

                var filePickerAttribute = prop.GetCustomAttribute<FilePickerAttribute>();
                if (filePickerAttribute != null)
                {
                    propertyViewModel.FilePicker = true;
                    propertyViewModel.PickFilter = filePickerAttribute.Type;
                }
                var folder = prop.GetCustomAttribute<FolderPickerAttribute>();
                propertyViewModel.FolderPicker = folder != null;

                if (prop.PropertyType.IsEnum)
                {
                    propertyViewModel.IsEnum = true;
                    propertyViewModel.EnumType = prop.PropertyType;
                    var enumValues = Enum.GetValues(prop.PropertyType);
                    propertyViewModel.EnumValues = new ObservableCollection<string>();
                    foreach (var enumValue in enumValues)
                    {
                        propertyViewModel.EnumValues.Add(enumValue.ToString());
                    }
                    var currentValue = prop.GetValue(LayerModel);
                    if (currentValue != null)
                    {
                        propertyViewModel.Value = currentValue.ToString();
                    }
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
                    try
                    {
                        object convertedValue;
                        if (prop.PropertyType.IsEnum && propertyViewModel.Value is string enumString)
                        {
                            convertedValue = Enum.Parse(prop.PropertyType, enumString);
                        }
                        else
                        {
                            convertedValue = Convert.ChangeType(propertyViewModel.Value, prop.PropertyType);
                        }
                        prop.SetValue(LayerModel, convertedValue);
                    }
                    catch (Exception e)
                    {
                        Log.Debug("Conversion error: " + e.Message, e);
                        propertyViewModel.Value = prop.GetValue(LayerModel)?.ToString();
                    }
                }
            }
        }
    }
}
