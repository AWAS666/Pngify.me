﻿using CommunityToolkit.Mvvm.ComponentModel;
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
    }

    private List<Layersetting> _layers;

    public ObservableCollection<LayersettingViewModel> Layers { get; set; }

    public void AddNewSettings()
    {
        var set = new Layersetting();
        Layers.Add(new LayersettingViewModel(set, this));
        _layers.Add(set);
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
        SettingsManager.Current.LayerSetup.ApplySettings();
    }
}