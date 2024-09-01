using SkiaSharp;

namespace PngifyMe.Layers
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
        public float Opacity { get; set; } = 1f;

        public SKBitmap Image { get; set; }
    }
}
