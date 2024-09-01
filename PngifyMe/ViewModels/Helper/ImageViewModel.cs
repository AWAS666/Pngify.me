using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Services.ThrowingSystem;

namespace PngifyMe.ViewModels.Helper
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
        private IImage image;

        public MovableObject Item { get; }

        public ImageViewModel(Services.ThrowingSystem.MovableObject item)
        {
            Item = item;
        }
    }
}
