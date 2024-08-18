using CommunityToolkit.Mvvm.ComponentModel;
using PngTuberSharp.Services.Settings;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace PngTuberSharp.ViewModels.Helper
{
    public partial class TriggerEditorViewModel : ObservableObject
    {

        public TriggerEditorViewModel()
        {          
            TriggerTypes = new ObservableCollection<Type>(Assembly.GetExecutingAssembly()
                                    .GetTypes()
                                    .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Trigger))));

        }

        private Layersetting _layerSetting;
        public Layersetting LayerSetting
        {
            get => _layerSetting;
            set
            {
                SetProperty(ref _layerSetting, value);
                // Set the initial trigger when the Layersetting changes
                if (_layerSetting?.Trigger != null)
                {
                    SelectedTriggerType = _layerSetting.Trigger?.GetType();
                    SelectedTrigger = _layerSetting.Trigger;
                }
            }
        }

        // List of available trigger types
        public ObservableCollection<Type> TriggerTypes { get; }

        private Type _selectedTriggerType;
        public Type SelectedTriggerType
        {
            get => _selectedTriggerType;
            set
            {
                SetProperty(ref _selectedTriggerType, value);
                // When the trigger type changes, instantiate a new object and update properties
                SelectedTrigger = (Trigger)Activator.CreateInstance(value);
            }
        }

        private Trigger _selectedTrigger;
        public Trigger SelectedTrigger
        {
            get => _selectedTrigger;
            set
            {
                SetProperty(ref _selectedTrigger, value);
                UpdatePropertyList();
            }
        }

        [ObservableProperty]
        private ObservableCollection<PropertyViewModel> _propertyList = new ObservableCollection<PropertyViewModel>();

        private void UpdatePropertyList()
        {
            PropertyList.Clear();
            if (SelectedTrigger != null)
            {
                // Use reflection to get properties
                foreach (var prop in SelectedTrigger.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    // Ignore JSON ignored properties
                    if (Attribute.IsDefined(prop, typeof(JsonIgnoreAttribute)))
                        continue;

                    var propertyViewModel = new PropertyViewModel
                    {
                        Name = prop.Name,
                        Value = prop.GetValue(SelectedTrigger)?.ToString()
                    };
                    PropertyList.Add(propertyViewModel);
                }
            }
        }
    }
}
