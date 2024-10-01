using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;
using System.Collections.Generic;

namespace PngifyMe.Views.Helper
{
    /// <summary>
    /// this has a couple of issues:
    /// - doesnt auto fit to the parent size and keep proportions like an image does
    /// - double click onto this doesnt work
    /// - 
    /// </summary>
    public class SKImageViewer : UserControl
    {

        static SKImageViewer()
        {
            AffectsRender<SKImageViewer>(SourceProperty);
        }

        public SKImageViewer()
        {
            ClipToBounds = true;
        }


        public static readonly StyledProperty<SKBitmap> SourceProperty =
            AvaloniaProperty.Register<SKImageViewer, SKBitmap>(nameof(Source));
        private CustomDrawOp drawOp;
        private List<CustomDrawOp> oldFrame = new();

        public SKBitmap Source
        {
            get => GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public override void Render(DrawingContext context)
        {
            drawOp = new CustomDrawOp();
            drawOp.Update(new Rect(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height), Source);
            context.Custom(drawOp);
        }

        class CustomDrawOp : ICustomDrawOperation
        {
            public void Dispose()
            {
                image?.Dispose();
            }

            public Rect Bounds { get; private set; }

            private SKBitmap image;

            public void Update(Rect bounds, SKBitmap image)
            {
                this.image = image;
                Bounds = bounds;
            }

            public bool HitTest(Point p) => false;
            public bool Equals(ICustomDrawOperation other) => false;
            public void Render(ImmediateDrawingContext context)
            {
                var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();

                using var lease = leaseFeature.Lease();
                var canvas = lease.SkCanvas;

                if (image is SKBitmap bitmap)
                {
                    canvas.DrawBitmap(bitmap, SKRect.Create((float)Bounds.X, (float)Bounds.Y, (float)Bounds.Width, (float)Bounds.Height));
                }
            }

        }
    }
}
