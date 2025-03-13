using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Layers;
using PngifyMe.Services;
using PngifyMe.Services.Settings;
using PngifyMe.ViewModels.Helper;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PngifyMe.ViewModels;

public partial class LayerSetupViewModel : ObservableObject
{

    public LayerSetupViewModel() : this(new List<Layersetting>())
    { }
    public LayerSetupViewModel(List<Layersetting> layers)
    {
        _layers = layers;
        Layers = new ObservableCollection<LayersettingViewModel>(layers.Select(x => new LayersettingViewModel(x, this)));
        Selected = Layers.FirstOrDefault();
    }

    private List<Layersetting> _layers;

    [ObservableProperty]
    private LayersettingViewModel selected;

    public ObservableCollection<LayersettingViewModel> Layers { get; set; }

    public void AddNewSettings()
    {
        var set = new Layersetting();
        Layers.Add(new LayersettingViewModel(set, this));
        _layers.Add(set);
        Selected = Layers.Last();
    }

    public void RemoveCommand(LayersettingViewModel vm)
    {
        Layers.Remove(vm);
        vm.LayerSettModel.Cleanup();
        _layers.Remove(vm.LayerSettModel);
    }

    public void Save()
    {
        foreach (var layer in Layers)
        {
            layer.Save();
        }
        SettingsManager.Save();
        LayerManager.Pause();
        SettingsManager.Current.LayerSetup.ApplySettings();
        LayerManager.UnPause();
    }
}