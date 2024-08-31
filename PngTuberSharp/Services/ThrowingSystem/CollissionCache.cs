using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngTuberSharp.Services.ThrowingSystem
{
    public static class CollissionCache
    {
        private static Dictionary<SKBitmap, CollisionDetector> cache = new Dictionary<SKBitmap, CollisionDetector>();

        public static CollisionDetector GetAndCache(SKBitmap bitmap, int details)
        {
            if (cache.TryGetValue(bitmap, out CollisionDetector? detector))
                return new CollisionDetector(bitmap, detector.leftOutline, detector.rightOutline, details);
            else
            {
                var newValue = new CollisionDetector(bitmap, details);
                cache.Add(bitmap, newValue);
                return newValue;
            }
        }
    }
}
