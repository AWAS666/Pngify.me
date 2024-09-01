﻿using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

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
                return new AvaloniaImage(SKImage.FromBitmap(bitmap));
            }
            return default;
        }

        public static IImage? ToAvaloniaImage(this SKImage? bitmap)
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
        private SKImage? _source;
        SKBitmapDrawOperation? _drawImageOperation;

        public AvaloniaImage(SKImage? source)
        {
            _source = source;
            if (source?.Info.Size is SKSizeI size)
            {
                Size = new(size.Width, size.Height);
            }
        }

        public void UpdateImage(SKImage newBitmap)
        {
            if (newBitmap?.Info.Size is SKSizeI size)
            {
                Size = new(size.Width, size.Height);
            }
            if (_drawImageOperation != null)
            {
                var next = _drawImageOperation.NextBitmap;              
                foreach (var item in next.SkipLast(1).ToList())
                {
                    item.Dispose();
                    next.Remove(item);
                }
                _drawImageOperation.NextBitmap.Add(newBitmap);               
            }
        }

        public Size Size { get; set; }

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
            _drawImageOperation.Bounds = destRect;
            context.Custom(_drawImageOperation);
        }
    }

    public record class SKBitmapDrawOperation : ICustomDrawOperation
    {
        private bool rendering;
        public Rect Bounds { get; set; }

        public SKImage? Bitmap { get; set; }
        public List<SKImage> NextBitmap { get; set; } = new();

        public void Dispose()
        {

        }

        public bool Equals(ICustomDrawOperation? other) => false;

        public bool HitTest(Point p) => Bounds.Contains(p);

        public void Render(ImmediateDrawingContext context)
        {
            SKImage old = null;
            if (NextBitmap.Count > 0)
            {
                old = Bitmap;
                Bitmap = NextBitmap.Last();
                NextBitmap.Remove(Bitmap);
            }
            if (Bitmap is SKImage bitmap && context.PlatformImpl.GetFeature<ISkiaSharpApiLeaseFeature>() is ISkiaSharpApiLeaseFeature leaseFeature)
            {
                ISkiaSharpApiLease lease = leaseFeature.Lease();
                using (lease)
                {
                    lease.SkCanvas.DrawImage(bitmap, SKRect.Create((float)Bounds.X, (float)Bounds.Y, (float)Bounds.Width, (float)Bounds.Height));
                }
            }
            GC.KeepAlive(old);
            GC.KeepAlive(Bitmap);
            old?.Dispose();
        }
    }
}
