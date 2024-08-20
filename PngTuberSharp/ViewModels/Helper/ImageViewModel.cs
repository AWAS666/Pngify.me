using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json.Linq;
using PngTuberSharp.Layers;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace PngTuberSharp.ViewModels.Helper
{
    public partial class ImageViewModel : ObservableObject
    {
        [ObservableProperty]
        private float x;

        [ObservableProperty]
        private float y;

        [ObservableProperty]
        private float rotation;

        [ObservableProperty]
        private Bitmap image;

        public ImageViewModel()
        {

        }

    }

    


}
