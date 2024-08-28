using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PngTuberSharp.Helpers
{
    static class SkiaExtensions
    {
        public static SKBitmap? ToSKBitmap(this System.IO.Stream? stream)
        {
            if (stream == null)
                return null;
            return SKBitmap.Decode(stream);
        }

        public static IImage? ToAvaloniaImage(this SKBitmap? bitmap)
        {
            if (bitmap is not null)
            {
                return new AvaloniaImage(bitmap);
            }
            return default;
        }
    }
    public class AvaloniaImage : IImage, IDisposable
    {
        private SKBitmap? _source;
        SKBitmapDrawOperation? _drawImageOperation;
        private SKBitmap _next;

        public AvaloniaImage(SKBitmap? source)
        {
            _source = source;
            if (source?.Info.Size is SKSizeI size)
            {
                Size = new(size.Width, size.Height);
            }
        }

        public void UpdateImage(SKBitmap newBitmap)
        {
            if (_drawImageOperation != null)
            {
                _drawImageOperation.NextBitmap.Add(newBitmap);
            }
        }

        public Size Size { get; }

        public void Dispose() => _source?.Dispose();

        public void Draw(DrawingContext context, Rect sourceRect, Rect destRect)
        {
            if (_drawImageOperation is null)
            {
                _drawImageOperation = new SKBitmapDrawOperation()
                {
                    Bitmap = _source,
                };
            };
            _drawImageOperation.Bounds = sourceRect;
            context.Custom(_drawImageOperation);
        }

        public override bool Equals(object? obj)
        {
            return false;
        }
    }

    public record class SKBitmapDrawOperation : ICustomDrawOperation
    {
        private bool rendering;

        public Rect Bounds { get; set; }

        public SKBitmap? Bitmap { get; set; }
        public List<SKBitmap> NextBitmap { get; set; } = new();

        public void Dispose()
        {

        }

        public bool Equals(ICustomDrawOperation? other) => false;

        public bool HitTest(Point p) => Bounds.Contains(p);

        public void Render(ImmediateDrawingContext context)
        {
            if (rendering) return;
            rendering = true;
            if (NextBitmap.Count > 0)
            {
                Bitmap?.Dispose();
                Bitmap = NextBitmap.Last();
                for (int i = 0; i < NextBitmap.Count - 1; i++)
                {
                    NextBitmap[i].Dispose();
                }

                NextBitmap.Clear();
            }
            if (Bitmap is SKBitmap bitmap && context.PlatformImpl.GetFeature<ISkiaSharpApiLeaseFeature>() is ISkiaSharpApiLeaseFeature leaseFeature)
            {
                ISkiaSharpApiLease lease = leaseFeature.Lease();
                using (lease)
                {
                    lease.SkCanvas.DrawBitmap(bitmap, SKRect.Create((float)Bounds.X, (float)Bounds.Y, (float)Bounds.Width, (float)Bounds.Height));
                }
            }
            rendering = false;
        }
    }
}
