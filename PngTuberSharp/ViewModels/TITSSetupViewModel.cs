using CommunityToolkit.Mvvm.ComponentModel;
using PngTuberSharp.Layers;
using System;

namespace PngTuberSharp.ViewModels;

public partial class TITSSetupViewModel : ObservableObject
{


    public TITSSetupViewModel()
    {

    }

    public void Trigger()
    {
        LayerManager.ThrowingSystem.Trigger(Random.Shared.Next(5, 10));
    }
}