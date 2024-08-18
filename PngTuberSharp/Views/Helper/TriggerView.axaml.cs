using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using PngTuberSharp.Services.Settings;
using PngTuberSharp.ViewModels.Helper;
using System;
using System.Windows.Input;

namespace PngTuberSharp.Views.Helper;

public partial class TriggerView : UserControl
{
    public TriggerView()
    {
        InitializeComponent();
        DataContext = new TriggerViewModel(new HotkeyTrigger());
    }  
}