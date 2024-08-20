﻿using CommunityToolkit.Mvvm.ComponentModel;
using PngTuberSharp.Layers;
using PngTuberSharp.Services;
using PngTuberSharp.Services.Settings;
using PngTuberSharp.ViewModels.Helper;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PngTuberSharp.ViewModels;

public partial class LayerSetupViewModel : ObservableObject
{

    public LayerSetupViewModel() : this(new List<Layersetting>())
    { }
    public LayerSetupViewModel(List<Layersetting> layers)
    {
        _layers = layers;
        Layers = new ObservableCollection<LayersettingViewModel>(layers.Select(x => new LayersettingViewModel(x)));
    }

    private List<Layersetting> _layers;

    public ObservableCollection<LayersettingViewModel> Layers { get; set; }

    public void AddNewSettings()
    {
        var set = new Layersetting();
        Layers.Add(new LayersettingViewModel(set));
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