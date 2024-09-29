using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Layers;
using PngifyMe.Services.Settings;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace PngifyMe.ViewModels.Helper
{
    public partial class LayersettingViewModel : ObservableObject
    {

        /// <summary>
        /// just for preview in designer
        /// </summary>
        public LayersettingViewModel() : this(new Layersetting(), new LayerSetupViewModel())
        {
        }

        public LayersettingViewModel(Layersetting layerSett, LayerSetupViewModel parent)
        {
            TriggerTypes = new ObservableCollection<Type>(Assembly.GetExecutingAssembly()
                                    .GetTypes()
                                    .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Trigger))));

            AllLayers = new ObservableCollection<Type>(Assembly.GetExecutingAssembly()
                                    .GetTypes()
                                    .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BaseLayer))));

            LayerSettModel = layerSett;
            _selectedTriggerType = LayerSettModel.Trigger?.GetType();
            SelectedTrigger = LayerSettModel.Trigger;
            Name = layerSett.Name;
            layers = new ObservableCollection<BaseLayerViewModel>(layerSett.Layers.Select(x => new BaseLayerViewModel(x, this)));
            Parent = parent;
        }

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private ObservableCollection<BaseLayerViewModel> layers;

        public LayerSetupViewModel Parent { get; }
        public Layersetting LayerSettModel { get; }

        // List of available trigger types
        public ObservableCollection<Type> TriggerTypes { get; }
        public ObservableCollection<Type> AllLayers { get; }

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
                LayerSettModel.Trigger = value;
                SelectedTriggerView = new TriggerViewModel(value);
            }
        }

        [ObservableProperty]
        private TriggerViewModel selectedTriggerView;

        [ObservableProperty]
        private Type selectedLayer;

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
                        Value = prop.PropertyType == typeof(bool) ? (bool)prop.GetValue(SelectedTrigger) : prop.GetValue(SelectedTrigger)?.ToString(),
                        Type = prop.PropertyType
                    };
                    PropertyList.Add(propertyViewModel);
                }
            }
        }       

        public void AddNewLayer()
        {
            if (SelectedLayer == null)
                return;
            AddNewLayer(SelectedLayer);
            SelectedLayer = null;
        }

        public void AddNewLayer(Type selected)
        {
            var newLayer = (BaseLayer)Activator.CreateInstance(selected);
            Layers.Add(new BaseLayerViewModel(newLayer, this));
            LayerSettModel.Layers.Add(newLayer);
        }

        public void RemoveCommand(BaseLayerViewModel vm)
        {
            Layers.Remove(vm);
            LayerSettModel.Layers.Remove(vm.LayerModel);
        }

        public void Save()
        {
            LayerSettModel.Name = Name;
            foreach (var layer in Layers)
            {
                layer.Save();
            }
        }
    }
}
