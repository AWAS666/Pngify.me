using PngifyMe.Layers.Helper;
using SkiaSharp;
using System;

namespace PngifyMe.Layers.Image
{
    /// <summary>
    /// merely a sample, will be to intensive to render on cpu at 1920x1080
    /// </summary>
    [LayerDescription("ColorShift")]
    public class ColorShift : BitMapTransformer
    {
        [Unit("degrees (-180 to 180)")]
        public float HueShift { get; set; } = 0f;

        [Unit("percent (-100 to 100)")]
        public float SaturationShift { get; set; } = 0f;

        [Unit("percent (-100 to 100)")]
        public float ValueShift { get; set; } = 0f;

        [Unit("degrees per second")]
        public float HueShiftVelocity { get; set; } = 0f;

        [Unit("percent per second")]
        public float SaturationShiftVelocity { get; set; } = 0f;

        [Unit("percent per second")]
        public float ValueShiftVelocity { get; set; } = 0f;

        private float currentHueShift;
        private float currentSaturationShift;
        private float currentValueShift;
        private SKBitmap? lastReplace;
        private SKBitmap? lastRender;
        private float lastHueShift = float.MinValue;
        private float lastSaturationShift = float.MinValue;
        private float lastValueShift = float.MinValue;

        public override void OnCalculateParameters(float dt, ref LayerValues values)
        {
            if (HueShiftVelocity != 0f)
            {
                currentHueShift = (currentHueShift + HueShiftVelocity * dt) % 360f;
                if (currentHueShift < 0) currentHueShift += 360f;
            }
            else
            {
                currentHueShift = HueShift;
            }

            if (SaturationShiftVelocity != 0f)
            {
                currentSaturationShift += SaturationShiftVelocity * dt;
            }
            else
            {
                currentSaturationShift = SaturationShift;
            }

            if (ValueShiftVelocity != 0f)
            {
                currentValueShift += ValueShiftVelocity * dt;
            }
            else
            {
                currentValueShift = ValueShift;
            }
        }

        public override SKBitmap RenderBitmap(SKBitmap bitmap)
        {
            if (bitmap == null)
                return bitmap;

            float effectiveHueShift = currentHueShift / 360f;
            float effectiveSaturationShift = Math.Clamp(currentSaturationShift / 100f, -1f, 1f);
            float effectiveValueShift = Math.Clamp(currentValueShift / 100f, -1f, 1f);

            bool needsUpdate = lastReplace != bitmap ||
                Math.Abs(effectiveHueShift - lastHueShift) > 0.001f ||
                Math.Abs(effectiveSaturationShift - lastSaturationShift) > 0.001f ||
                Math.Abs(effectiveValueShift - lastValueShift) > 0.001f;

            if (!needsUpdate && lastRender != null)
                return lastRender;

            lastReplace = bitmap;
            lastHueShift = effectiveHueShift;
            lastSaturationShift = effectiveSaturationShift;
            lastValueShift = effectiveValueShift;

            if (Math.Abs(effectiveHueShift) < 0.001f && 
                Math.Abs(effectiveSaturationShift) < 0.001f && 
                Math.Abs(effectiveValueShift) < 0.001f)
            {
                lastRender?.Dispose();
                lastRender = bitmap.Copy();
                return lastRender;
            }

            var info = new SKImageInfo(bitmap.Width, bitmap.Height, SKColorType.Rgba8888, SKAlphaType.Premul);
            var output = new SKBitmap(info);

            using (var canvas = new SKCanvas(output))
            {
                using (var paint = new SKPaint())
                {
                    float[] colorMatrix = CreateHsvColorMatrix(effectiveHueShift, effectiveSaturationShift, effectiveValueShift);
                    paint.ColorFilter = SKColorFilter.CreateColorMatrix(colorMatrix);
                    canvas.DrawBitmap(bitmap, 0, 0, paint);
                }
            }

            lastRender?.Dispose();
            lastRender = output;
            return output;
        }

        private static float[] CreateHsvColorMatrix(float hShift, float sShift, float vShift)
        {
            float[] matrix = new float[20];
            
            if (Math.Abs(hShift) < 0.001f && Math.Abs(sShift) < 0.001f && Math.Abs(vShift) < 0.001f)
            {
                matrix[0] = 1; matrix[5] = 1; matrix[10] = 1; matrix[15] = 1;
                return matrix;
            }

            float h = hShift * 6.28318f;
            float cosH = MathF.Cos(h);
            float sinH = MathF.Sin(h);
            
            float s = 1f + sShift;
            float v = 1f + vShift;
            
            float lumR = 0.299f;
            float lumG = 0.587f;
            float lumB = 0.114f;
            
            float u = cosH;
            float w = sinH;
            
            matrix[0] = (lumR + (1f - lumR) * u - lumR * w) * v;
            matrix[1] = (lumG - lumG * u - lumG * w) * v;
            matrix[2] = (lumB - lumB * u + (1f - lumB) * w) * v;
            matrix[3] = 0;
            matrix[4] = 0;
            
            matrix[5] = (lumR - lumR * u + lumR * w) * v;
            matrix[6] = (lumG + (1f - lumG) * u - lumG * w) * v;
            matrix[7] = (lumB - lumB * u - lumB * w) * v;
            matrix[8] = 0;
            matrix[9] = 0;
            
            matrix[10] = (lumR - lumR * u - lumR * w) * v;
            matrix[11] = (lumG - lumG * u + lumG * w) * v;
            matrix[12] = (lumB + (1f - lumB) * u + lumB * w) * v;
            matrix[13] = 0;
            matrix[14] = 0;
            
            float rwgt = 0.3086f;
            float gwgt = 0.6094f;
            float bwgt = 0.0820f;
            
            matrix[0] = (1f - s) * rwgt + s * matrix[0];
            matrix[1] = (1f - s) * rwgt + s * matrix[1];
            matrix[2] = (1f - s) * rwgt + s * matrix[2];
            
            matrix[5] = (1f - s) * gwgt + s * matrix[5];
            matrix[6] = (1f - s) * gwgt + s * matrix[6];
            matrix[7] = (1f - s) * gwgt + s * matrix[7];
            
            matrix[10] = (1f - s) * bwgt + s * matrix[10];
            matrix[11] = (1f - s) * bwgt + s * matrix[11];
            matrix[12] = (1f - s) * bwgt + s * matrix[12];
            
            matrix[15] = 0;
            matrix[16] = 0;
            matrix[17] = 0;
            matrix[18] = 1;
            matrix[19] = 0;
            
            return matrix;
        }
    }
}
