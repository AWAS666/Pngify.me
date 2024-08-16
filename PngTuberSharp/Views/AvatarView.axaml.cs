using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using PngTuberSharp.Layers;
using PngTuberSharp.Services;
using PngTuberSharp.ViewModels;
using System;

namespace PngTuberSharp.Views;

public partial class AvatarView : UserControl
{
    public AvatarView()
    {
        InitializeComponent();
        DataContext = new AvatarViewModel();
    }
}