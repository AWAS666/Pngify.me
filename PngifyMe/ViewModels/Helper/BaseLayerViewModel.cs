using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Layers;
using PngifyMe.Layers.Helper;
using PngifyMe.Services;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Linq;
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
            HasCanvasOverlay = layer.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Any(p => p.GetCustomAttribute<CanvasPositionAttribute>() != null);
            UpdatePropertyList();
            Parent = parent;
        }

        [ObservableProperty]
        private ObservableCollection<PropertyViewModel> _propertyList = new ObservableCollection<PropertyViewModel>();
        [ObservableProperty]
        private string name;

        /// <summary>
        /// True if the layer has any property marked with <see cref="CanvasPositionAttribute"/> (canvas overlay can edit position/size).
        /// </summary>
        public bool HasCanvasOverlay { get; }

        [RelayCommand]
        private void OpenCanvasOverlay()
        {
            CanvasOverlayService.SetOverlay(new PositionSizeOverlayViewModel(this));
        }

        private void UpdatePropertyList()
        {
            PropertyList.Clear();
            var layerType = LayerModel.GetType();
            var hasPointProperty = layerType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Any(p => p.GetCustomAttribute<CanvasPositionAttribute>()?.IsPointProperty == true);
            foreach (var prop in layerType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var canvasPosAttr = prop.GetCustomAttribute<CanvasPositionAttribute>();
                if (canvasPosAttr != null && canvasPosAttr.IsPointProperty)
                {
                    var posObj = prop.GetValue(LayerModel);
                    var xVal = GetSubPropertyValue(posObj, "X");
                    var yVal = GetSubPropertyValue(posObj, "Y");
                    PropertyList.Add(new PropertyViewModel
                    {
                        Name = $"{prop.Name}.X",
                        Value = xVal?.ToString() ?? "0",
                        Unit = UnitNames.PixelsCenter,
                        Type = typeof(float),
                        ShowEditOnCanvasButton = true,
                        SourcePropertyName = prop.Name,
                        SourceSubPropertyName = "X",
                    });
                    PropertyList.Add(new PropertyViewModel
                    {
                        Name = $"{prop.Name}.Y",
                        Value = yVal?.ToString() ?? "0",
                        Unit = UnitNames.PixelsCenter,
                        Type = typeof(float),
                        SourcePropertyName = prop.Name,
                        SourceSubPropertyName = "Y",
                    });
                    continue;
                }
                if (Attribute.IsDefined(prop, typeof(JsonIgnoreAttribute)))
                    continue;
                if (hasPointProperty && (prop.Name == "PosX" || prop.Name == "PosY"))
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

                var canvasPos = prop.GetCustomAttribute<CanvasPositionAttribute>();
                if (canvasPos != null && !canvasPos.IsPointProperty && canvasPos.Role == CanvasPositionRole.X)
                    propertyViewModel.ShowEditOnCanvasButton = true;

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

        private static object? GetSubPropertyValue(object? obj, string subName)
        {
            if (obj == null) return null;
            var sub = obj.GetType().GetProperty(subName, BindingFlags.Public | BindingFlags.Instance);
            return sub?.GetValue(obj);
        }

        public void Save()
        {
            foreach (var propertyViewModel in PropertyList)
            {
                if (!string.IsNullOrEmpty(propertyViewModel.SourcePropertyName) && !string.IsNullOrEmpty(propertyViewModel.SourceSubPropertyName))
                {
                    var prop = LayerModel.GetType().GetProperty(propertyViewModel.SourcePropertyName, BindingFlags.Public | BindingFlags.Instance);
                    if (prop == null) continue;
                    var posObj = prop.GetValue(LayerModel);
                    if (posObj == null)
                    {
                        posObj = Activator.CreateInstance(prop.PropertyType);
                        if (posObj == null) continue;
                    }
                    var subProp = posObj.GetType().GetProperty(propertyViewModel.SourceSubPropertyName, BindingFlags.Public | BindingFlags.Instance);
                    if (subProp != null)
                    {
                        try
                        {
                            var converted = Convert.ChangeType(propertyViewModel.Value, subProp.PropertyType);
                            subProp.SetValue(posObj, converted);
                            prop.SetValue(LayerModel, posObj);
                        }
                        catch (Exception e)
                        {
                            Log.Debug("Conversion error: " + e.Message, e);
                        }
                    }
                    continue;
                }
                var directProp = LayerModel.GetType().GetProperty(propertyViewModel.Name, BindingFlags.Public | BindingFlags.Instance);
                if (directProp != null)
                {
                    try
                    {
                        object convertedValue;
                        if (directProp.PropertyType.IsEnum && propertyViewModel.Value is string enumString)
                        {
                            convertedValue = Enum.Parse(directProp.PropertyType, enumString);
                        }
                        else
                        {
                            convertedValue = Convert.ChangeType(propertyViewModel.Value, directProp.PropertyType);
                        }
                        directProp.SetValue(LayerModel, convertedValue);
                    }
                    catch (Exception e)
                    {
                        Log.Debug("Conversion error: " + e.Message, e);
                        propertyViewModel.Value = directProp.GetValue(LayerModel)?.ToString();
                    }
                }
            }
        }
    }
}
