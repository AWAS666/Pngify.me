using PngifyMe.Layers.Helper;
using SkiaSharp;
using System;

namespace PngifyMe.Layers.Image
{
    [LayerDescription("ChromaticAberration")]
    public class ChromaticAberration : BitMapTransformer
    {
        [Unit("pixels")]
        public float Strength { get; set; } = 2f;

        private SKBitmap? lastReplace;
        private SKBitmap? lastRender;
        private float lastStrength = float.MinValue;

        public override void OnCalculateParameters(float dt, ref LayerValues values)
        {
            // nothing
        }

        public override SKBitmap RenderBitmap(SKBitmap bitmap)
        {
            if (bitmap == null)
                return bitmap;

            if (lastReplace == bitmap && Math.Abs(Strength - lastStrength) < 0.01f && lastRender != null)
                return lastRender;

            lastReplace = bitmap;
            lastStrength = Strength;

            if (Math.Abs(Strength) < 0.01f)
            {
                lastRender?.Dispose();
                lastRender = bitmap.Copy();
                return lastRender;
            }

            var info = new SKImageInfo(bitmap.Width, bitmap.Height, SKColorType.Rgba8888, SKAlphaType.Premul);
            var output = new SKBitmap(info);

            unsafe
            {
                int width = bitmap.Width;
                int height = bitmap.Height;
                int srcRowBytes = bitmap.Info.RowBytes;
                int dstRowBytes = output.Info.RowBytes;

                byte* srcPixels = (byte*)bitmap.GetPixels();
                byte* dstPixels = (byte*)output.GetPixels();

                bool isBgra = bitmap.Info.ColorType == SKColorType.Bgra8888;
                if (!isBgra && bitmap.Info.ColorType != SKColorType.Rgba8888)
                {
                    return bitmap;
                }

                int centerX = width / 2;
                int centerY = height / 2;
                float maxDist = MathF.Sqrt(centerX * centerX + centerY * centerY);
                float invMaxDist = 1f / maxDist;
                float strengthNorm = Strength;

                for (int y = 0; y < height; y++)
                {
                    byte* dstRow = dstPixels + y * dstRowBytes;
                    float dy = (y - centerY);
                    float dySq = dy * dy;

                    for (int x = 0; x < width; x++)
                    {
                        int dstOffset = x * 4;
                        float dx = (x - centerX);
                        float dist = MathF.Sqrt(dx * dx + dySq);
                        float normalizedDist = dist * invMaxDist;

                        float offsetX = dx * normalizedDist * strengthNorm;
                        float offsetY = dy * normalizedDist * strengthNorm;

                        int redX = (int)Math.Clamp(x + offsetX, 0, width - 1);
                        int redY = (int)Math.Clamp(y + offsetY, 0, height - 1);
                        int blueX = (int)Math.Clamp(x - offsetX, 0, width - 1);
                        int blueY = (int)Math.Clamp(y - offsetY, 0, height - 1);

                        byte* srcRowRed = srcPixels + redY * srcRowBytes;
                        byte* srcRowBlue = srcPixels + blueY * srcRowBytes;
                        byte* srcRowGreen = srcPixels + y * srcRowBytes;

                        int redOffset = redX * 4;
                        int blueOffset = blueX * 4;
                        int greenOffset = dstOffset;

                        byte r, g, b, a;

                        if (isBgra)
                        {
                            r = srcRowRed[redOffset + 2];
                            g = srcRowGreen[greenOffset + 1];
                            b = srcRowBlue[blueOffset + 0];
                            a = srcRowGreen[greenOffset + 3];
                        }
                        else
                        {
                            r = srcRowRed[redOffset + 0];
                            g = srcRowGreen[greenOffset + 1];
                            b = srcRowBlue[blueOffset + 2];
                            a = srcRowGreen[greenOffset + 3];
                        }

                        dstRow[dstOffset + 0] = r;
                        dstRow[dstOffset + 1] = g;
                        dstRow[dstOffset + 2] = b;
                        dstRow[dstOffset + 3] = a;
                    }
                }
            }

            lastRender?.Dispose();
            lastRender = output;
            return output;
        }

    }
}
