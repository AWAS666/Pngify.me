using PngifyMe.Layers.Helper;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace PngifyMe.Layers.Image;

[LayerDescription("Render as terminal")]
public class TextRenderer : BitMapTransformer
{
    [Unit("")]
    public string Text { get; set; } = "#";

    [Unit("pixel")]
    public int Height { get; set; } = 16;

    [Unit("pixel")]
    public int Width { get; set; } = 8;

    private TerminalRenderer renderer = new();
    private SKBitmap lastReplace;
    private SKBitmap lastRender;

    public override void OnEnter()
    {
        if (string.IsNullOrEmpty(Text))
            Text = "#";
        base.OnEnter();
    }

    public override void OnCalculateParameters(float dt, ref LayerValues values)
    {
        //nothing
    }

    public override SKBitmap RenderBitmap(SKBitmap bitmap)
    {
        // skip render if last is same
        if (lastReplace == bitmap)
            return lastRender;

        lastReplace = bitmap;
        lastRender = renderer.RenderTerminalMode(bitmap, Text, cellHeight: Height, cellWidth: Width);
        return lastRender;
    }
}


public unsafe class TerminalRenderer
{
    /// <summary>
    /// Renders the given bitmap into "terminal mode" by replacing image blocks with characters.
    /// Each cell is rendered with a character (cycling through the provided string) drawn in the
    /// average colour (including alpha) of that block. Pixel iteration is done using unsafe code
    /// for improved performance.
    /// This version uses consistent baselines computed via font metrics.
    /// </summary>
    /// <param name="source">The source bitmap.</param>
    /// <param name="charSequence">
    /// A string containing one or more characters to use. The characters are cycled in order.
    /// </param>
    /// <param name="cellWidth">Width of each cell in pixels.</param>
    /// <param name="cellHeight">Height of each cell in pixels (also used as text size).</param>
    /// <param name="alphaThreshold">Minimum average alpha (0-255) required to draw the character.</param>
    /// <param name="colourAccuracy">1 checks every pixel for colour to average, 2 only every other, etc.</param>
    /// <returns>A new bitmap with the terminal effect applied.</returns>
    public SKBitmap RenderTerminalMode(
        SKBitmap source,
        string charSequence,
        int cellWidth = 8,
        int cellHeight = 16,
        byte alphaThreshold = 128,
        int colourAccuracy = 8)
    {
        if (string.IsNullOrEmpty(charSequence))
            throw new ArgumentException("charSequence must not be null or empty.", nameof(charSequence));

        // Ensure the output bitmap supports transparency.
        var info = new SKImageInfo(source.Width, source.Height, SKColorType.Rgba8888, SKAlphaType.Premul);
        SKBitmap output = new SKBitmap(info);

        // Precompute each unique character's width for horizontal centering.
        Dictionary<char, float> charWidths = new Dictionary<char, float>();

        using var font = new SKFont(SKTypeface.Default, cellHeight);
        using var paint = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill };

        // Use font.Metrics property to get font metrics.
        SKFontMetrics metrics = font.Metrics;
        // The effective text height.
        float textHeight = metrics.Descent - metrics.Ascent;

        // Precompute widths for each unique character.
        foreach (char c in charSequence)
        {
            if (!charWidths.ContainsKey(c))
            {
                float width = font.MeasureText(c.ToString());
                charWidths[c] = width;
            }
        }

        using var surface = SKSurface.Create(info, output.GetPixels());
        SKCanvas canvas = surface.Canvas;
        // Clear canvas to transparent.
        canvas.Clear(SKColors.Transparent);

        // this takes quite some performance...
        //using SKPaint alpha = new SKPaint { Color = SKColors.White.WithAlpha(15) };
        //canvas.DrawBitmap(source, 0, 0, alpha);

        int cols = source.Width / cellWidth;
        int rows = source.Height / cellHeight;
        int charCount = charSequence.Length;
        int cellIndex = 0;

        int srcWidth = source.Width;
        int srcHeight = source.Height;
        int rowBytes = source.Info.RowBytes;
        byte* srcPixels = (byte*)source.GetPixels();

        // Determine pixel ordering.
        bool isBgra = source.Info.ColorType == SKColorType.Bgra8888;
        bool isRgba = source.Info.ColorType == SKColorType.Rgba8888;
        if (!isBgra && !isRgba)
            throw new NotSupportedException("Only BGRA8888 and RGBA8888 are supported in unsafe mode.");

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int startX = col * cellWidth;
                int startY = row * cellHeight;

                long sumR = 0, sumG = 0, sumB = 0, sumA = 0;
                int pixelCount = 0;

                for (int y = startY; y < startY + cellHeight && y < srcHeight; y += colourAccuracy)
                {
                    byte* rowPtr = srcPixels + y * rowBytes;
                    for (int x = startX; x < startX + cellWidth && x < srcWidth; x += colourAccuracy)
                    {
                        int offset = x * 4;
                        byte r, g, b, a;
                        if (isBgra)
                        {
                            // BGRA ordering.
                            b = rowPtr[offset + 0];
                            g = rowPtr[offset + 1];
                            r = rowPtr[offset + 2];
                            a = rowPtr[offset + 3];
                        }
                        else // if (isRgba)
                        {
                            r = rowPtr[offset + 0];
                            g = rowPtr[offset + 1];
                            b = rowPtr[offset + 2];
                            a = rowPtr[offset + 3];
                        }
                        sumR += r;
                        sumG += g;
                        sumB += b;
                        sumA += a;
                        pixelCount++;
                    }
                }

                if (pixelCount == 0)
                    continue;

                byte avgA = (byte)(sumA / pixelCount);
                if (avgA < alphaThreshold)
                {
                    cellIndex++;
                    continue;
                }

                byte avgR = (byte)(sumR / pixelCount);
                byte avgG = (byte)(sumG / pixelCount);
                byte avgB = (byte)(sumB / pixelCount);
                SKColor avgColor = new SKColor(avgR, avgG, avgB, avgA);

                paint.Color = avgColor;

                // Choose character from the sequence.
                char c = charSequence[cellIndex % charCount];
                cellIndex++;
                string text = c.ToString();
                float textWidth = charWidths[c];

                // Compute horizontal center.
                float posX = startX + (cellWidth - textWidth) / 2f;
                // Compute vertical baseline using font metrics:
                // baseline = cellTop + (cellHeight - textHeight) / 2 - metrics.Ascent
                float posY = startY + (cellHeight - textHeight) / 2f - metrics.Ascent;

                canvas.DrawText(text, posX, posY, font, paint);
            }
        }

        surface.Flush();

        return output;
    }
}
