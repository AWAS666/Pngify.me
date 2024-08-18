using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using PngTuberSharp.Services.Settings;
using PngTuberSharp.ViewModels.Helper;
using System;

namespace PngTuberSharp.Views.Helper;

public partial class TriggerEditorViewProxy : UserControl
{
    public TriggerEditorViewProxy()
    {
        InitializeComponent();
        viewModel = new TriggerEditorViewModel();
        view.DataContext = viewModel;
    }

    public static readonly DirectProperty<TriggerEditorViewProxy, Layersetting> LayerSettingProperty =
        AvaloniaProperty.RegisterDirect<TriggerEditorViewProxy, Layersetting>(
            nameof(LayerSetting),
            o => o.LayerSetting,
            (o, v) => o.LayerSetting = v,
            defaultBindingMode: BindingMode.TwoWay);


    private Layersetting _items;
    private TriggerEditorViewModel viewModel;

    public Layersetting LayerSetting
    {
        get { return _items; }
        set
        {
            SetAndRaise(LayerSettingProperty, ref _items, value);
            if (value != null)
                viewModel.LayerSetting = value;
        }
    }
}