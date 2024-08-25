using CommunityToolkit.Mvvm.ComponentModel;
using PngTuberSharp.Layers;
using PngTuberSharp.Services;
using PngTuberSharp.Services.Settings;
using PngTuberSharp.ViewModels.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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