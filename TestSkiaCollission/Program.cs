﻿using SkiaSharp;
using System.Diagnostics;

namespace TestSkiaCollission
{

    class SimpleCollisionDetector
    {
        private SKBitmap bitmap;
        private List<SKPoint> leftOutline;
        private List<SKPoint> rightOutline;
        private int details;
        public SKPoint Offset { get; set; } = new SKPoint(0, 0);

        public SimpleCollisionDetector(SKBitmap bitmap, int details)
        {
            this.bitmap = bitmap;
            this.leftOutline = new List<SKPoint>();
            this.rightOutline = new List<SKPoint>();
            this.details = details;
            GenerateOutlines();
        }

        private void GenerateOutlines()
        {
            // Trace the left outline
            for (int y = 0; y < bitmap.Height; y += details)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    if (bitmap.GetPixel(x, y).Alpha != 0)
                    {
                        leftOutline.Add(new SKPoint(x, y));
                        break; // move to the next row after finding the first non-transparent pixel
                    }
                }
            }

            // Trace the right outline
            for (int y = 0; y < bitmap.Height; y += details)
            {
                for (int x = bitmap.Width - 1; x >= 0; x--)
                {
                    if (bitmap.GetPixel(x, y).Alpha != 0)
                    {
                        rightOutline.Add(new SKPoint(x, y));
                        break; // move to the next row after finding the first non-transparent pixel
                    }
                }
            }
        }

        public void DrawOutlines(SKCanvas canvas, SKPaint paint)
        {
            // Draw left outline
            for (int i = 0; i < leftOutline.Count - 1; i++)
            {
                canvas.DrawLine(leftOutline[i], leftOutline[i + 1], paint);
            }

            // Draw right outline
            for (int i = 0; i < rightOutline.Count - 1; i++)
            {
                canvas.DrawLine(rightOutline[i], rightOutline[i + 1], paint);
            }

            // Connect the two outlines
            if (leftOutline.Count > 0 && rightOutline.Count > 0)
            {
                canvas.DrawLine(leftOutline[0], rightOutline[0], paint);
                canvas.DrawLine(leftOutline[^1], rightOutline[^1], paint);
            }
        }

        public bool CollidesWith(SimpleCollisionDetector other)
        {
            return CheckCollisionOutline(leftOutline, rightOutline, this.Offset, other.leftOutline, other.rightOutline, other.Offset);
        }

        private bool CheckCollisionOutline(List<SKPoint> outline1, List<SKPoint> outline2, SKPoint offset1, List<SKPoint> otherLeft, List<SKPoint> otherRight, SKPoint offset2)
        {
            // Apply offsets to both outlines
            foreach (var point in outline1)
            {
                SKPoint translatedPoint = new SKPoint(point.X + offset1.X, point.Y + offset1.Y);
                if (IsPointInOutline(translatedPoint, otherLeft, otherRight, offset2))
                    return true;
            }

            foreach (var point in outline2)
            {
                SKPoint translatedPoint = new SKPoint(point.X + offset1.X, point.Y + offset1.Y);
                if (IsPointInOutline(translatedPoint, otherLeft, otherRight, offset2))
                    return true;
            }

            return false;
        }

        private bool IsPointInOutline(SKPoint point, List<SKPoint> left, List<SKPoint> right, SKPoint offset)
        {
            int intersections = 0;
            float testX = point.X;
            float testY = point.Y;

            // Check intersections with left outline
            for (int i = 0; i < left.Count - 1; i++)
            {
                SKPoint p1 = new SKPoint(left[i].X + offset.X, left[i].Y + offset.Y); // Apply offset to the other object's outline
                SKPoint p2 = new SKPoint(left[i + 1].X + offset.X, left[i + 1].Y + offset.Y); // Apply offset to the other object's outline
                if (DoLinesIntersect(testX, testY, p1, p2))
                    intersections++;
            }

            // Check intersections with right outline
            for (int i = 0; i < right.Count - 1; i++)
            {
                SKPoint p1 = new SKPoint(right[i].X + offset.X, right[i].Y + offset.Y); // Apply offset to the other object's outline
                SKPoint p2 = new SKPoint(right[i + 1].X + offset.X, right[i + 1].Y + offset.Y); // Apply offset to the other object's outline
                if (DoLinesIntersect(testX, testY, p1, p2))
                    intersections++;
            }

            // Point is inside if intersections count is odd
            return intersections % 2 != 0;
        }

        private bool DoLinesIntersect(float x, float y, SKPoint p1, SKPoint p2)
        {
            if ((y >= Math.Min(p1.Y, p2.Y) && y < Math.Max(p1.Y, p2.Y)) &&
                (x < Math.Max(p1.X, p2.X)) &&
                (p1.Y != p2.Y))
            {
                float xIntersection = p1.X + (y - p1.Y) / (p2.Y - p1.Y) * (p2.X - p1.X);
                if (x < xIntersection)
                    return true;
            }
            return false;
        }

        public static void Main(string[] args)
        {
            for (int i = 1; i <= 1000; i++)
            {
                using (var stream = System.IO.File.OpenRead("image.png"))
                using (var bitmap = SKBitmap.Decode(stream))
                using (var surface = SKSurface.Create(new SKImageInfo(bitmap.Width, bitmap.Height)))
                {
                    var watch = new Stopwatch();
                    watch.Start();
                    var collisionDetector = new SimpleCollisionDetector(bitmap, i);

                    Console.WriteLine($"{i} took {watch.Elapsed.TotalMilliseconds} ms");

                    var canvas = surface.Canvas;
                    canvas.Clear(SKColors.White);

                    using (var paint = new SKPaint
                    {
                        Color = SKColors.Black,
                        StrokeWidth = 1,
                        IsAntialias = true,
                        Style = SKPaintStyle.Stroke
                    })
                    {
                        collisionDetector.DrawOutlines(canvas, paint);
                    }

                    using (var image = surface.Snapshot())
                    using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                    using (var output = System.IO.File.OpenWrite($"output{i}.png"))
                    {
                        data.SaveTo(output);
                    }
                }
            }

            for (int i = 1; i <= 100; i++)
            {
                using var bitmap = SKBitmap.Decode("image.png");
                var collisionDetector = new SimpleCollisionDetector(bitmap, i);

                using var bitmap1 = SKBitmap.Decode("image.png");
                var other = new SimpleCollisionDetector(bitmap, i);
                other.Offset = new SKPoint(2000, 0);

                var watch = new Stopwatch();
                watch.Start();
                bool col = collisionDetector.CollidesWith(other);
                Console.WriteLine($"{i} took {watch.Elapsed.TotalMilliseconds} ms, collission: {col}");
            }

        }
    }

}