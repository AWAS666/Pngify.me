using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngifyMe.ViewModels;

public partial class SplashScreenViewModel : ObservableObject
{
    [ObservableProperty]
    private string text = "Loading...";
}
