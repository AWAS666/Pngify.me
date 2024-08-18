using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using PngTuberSharp.Layers;
using PngTuberSharp.Layers.Microphone;
using PngTuberSharp.Services;
using PngTuberSharp.Services.Settings;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PngTuberSharp.ViewModels;

public partial class LayerSetupViewModel : ObservableObject
{
    public ObservableCollection<Layersetting> PermaLayers { get; set; } =
        new ObservableCollection<Layersetting>(SettingsManager.Current.LayerSetup.Layers.FindAll(x => x.Trigger is AlwaysActive));
    public ObservableCollection<Layersetting> TriggeredLayers { get; set; } =
        new ObservableCollection<Layersetting>(SettingsManager.Current.LayerSetup.Layers.FindAll(x => !(x.Trigger is AlwaysActive)));

    public void AddNewSettings()
    {
        var set = new Layersetting();
        PermaLayers.Add(set);
    }

    public void Remove(object sender, RoutedEventArgs e)
    {
    }

    public string Name { get; set; } = "test";
}
