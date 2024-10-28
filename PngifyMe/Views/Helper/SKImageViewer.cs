using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Reactive;
using SkiaSharp;

namespace PngifyMe.Views.Helper
{
    /// <summary>
    /// need to dispose bitmaps outside if updated regularly
    /// </summary>
    public class SKImageViewer : UserControl
    {

        public static readonly StyledProperty<SKBitmap> SourceProperty =
            AvaloniaProperty.Register<SKImageViewer, SKBitmap>(nameof(Source));

        static SKImageViewer()
        {
            AffectsRender<SKImageViewer>(SourceProperty);
        }

        public SKImageViewer()
        {
            ClipToBounds = true;

            SourceProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<SKBitmap>>(
               e =>
               {
                   base.InvalidateMeasure();
                   base.InvalidateVisual();
               })
            );
        }

        private Size RenderSize =>
          this.Bounds.Size;
        private WriteableBitmap writableBitmap;

        protected override Size MeasureOverride(Size constraint) =>
           this.InternalMeasureArrangeOverride(constraint);

        protected override Size ArrangeOverride(Size arrangeSize) =>
            this.InternalMeasureArrangeOverride(arrangeSize);

        private Size InternalMeasureArrangeOverride(Size targetSize)
        {
            if (Source != null)
            {
                var self = new Size(Source.Width, Source.Height);
                var scaleFactor = ComputeScaleFactor(
                    targetSize,
                    self)
                  ;
                return new(
                   self.Width * scaleFactor.Width,
                   self.Height * scaleFactor.Height);
            }
            else
            {
                return default;
            }
        }


        public SKBitmap Source
        {
            get => GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public override void Render(DrawingContext drawingContext)
        {
            base.Render(drawingContext);
            if (Source == null) return;

            int width = Source.Width;
            int height = Source.Height;

            var info = new SKImageInfo(
                width, height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);

            writableBitmap = writableBitmap ?? new WriteableBitmap(
                 new(info.Width, info.Height), new(96.0, 96.0), PixelFormat.Bgra8888, AlphaFormat.Premul);
            using var locker = writableBitmap.Lock();
            using var surface = SKSurface.Create(info, locker.Address, locker.RowBytes);
            surface.Canvas.Clear();
            surface.Canvas.DrawBitmap(Source, default(SKPoint));
            drawingContext.DrawImage(writableBitmap, new(new(), this.RenderSize));
        }

        private Size ComputeScaleFactor(Size availableSize, Size contentSize)
        {
            // Compute scaling factors to use for axes
            double scaleX = 1.0;
            double scaleY = 1.0;

            // Compute scaling factors for both axes
            scaleX = availableSize.Width / contentSize.Width;
            scaleY = availableSize.Height / contentSize.Height;

            //Find maximum scale that we use for both axes
            double minscale = scaleX < scaleY ? scaleX : scaleY;
            scaleX = scaleY = minscale;

            //Return this as a size now
            return new Size(scaleX, scaleY);
        }

    }
}
