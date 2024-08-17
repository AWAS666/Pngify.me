using Avalonia.Media.Imaging;

namespace PngTuberSharp.Layers
{
    public struct LayerValues
    {
        public LayerValues()
        {

        }

        public float PosX { get; set; }
        public float PosY { get; set; }
        public float Rotation { get; set; }
        public float Zoom
        {
            set
            {
                ZoomX = value;
                ZoomY = value;
            }
        }
        public float ZoomX { get; set; } = 1f;
        public float ZoomY { get; set; } = 1f;
        public double Opacity { get; set; } = 1f;

        public Bitmap Image { get; set; }
    }
}
