using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using PngTuberSharp.Layers;
using PngTuberSharp.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace PngTuberSharp.Views;

public partial class LayerSetup : UserControl
{
    public LayerSetup()
    {
        InitializeComponent();        
    }   
}