using PngifyMe.Layers.Helper;
using PngifyMe.Services;
using SkiaSharp;

namespace PngifyMe.Layers.Image
{
    [LayerDescription("ScrollingText")]
    public class ScrollingText : ImageLayer
    {
        private float currentOffset;
        private SKPaint paint;

        public string Text { get; set; } = "Your Text";

        [Unit("Pixels/s")]
        public int Pixel { get; set; } = 100;

        [Unit("Pixels")]
        public int TextSize { get; set; } = 100;

        public override void OnEnter()
        {
            paint = new SKPaint();
            paint.Color = SKColors.Black;
            paint.TextSize = TextSize;
            paint.IsAntialias = true;

            // Measure the width of the text
            float textWidth = paint.MeasureText(Text);
            currentOffset -= textWidth;
            base.OnEnter();
        }


        public override void OnCalculateParameters(float dt, ref LayerValues values)
        {
            currentOffset += Pixel * dt;
        }

        public override void RenderImage(SKCanvas canvas, float offsetX, float offsetY)
        {
            // Draw the text at the current position
            canvas.DrawText(Text, currentOffset + offsetX, Specsmanager.Height / 2 + offsetY, paint);

            if (currentOffset > Specsmanager.Width)
                IsExiting = true;
        }
    }
}
