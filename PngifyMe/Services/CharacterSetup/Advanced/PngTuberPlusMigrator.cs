using Avalonia.Media;
using NAudio.CoreAudioApi;
using PngifyMe.Services.CharacterSetup.Images;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ursa.Common;

namespace PngifyMe.Services.CharacterSetup.Advanced;

public static class PngTuberPlusMigrator
{
    public static SpriteImage MigratePngtuberPlus(PngTuberPlusObject self, List<PngTuberPlusObject> items, SpriteImage parent = null)
    {
        var sprite = new SpriteImage();
        sprite.Id = self.identification;
        sprite.ImageBase64 = [self.imageData];
        sprite.FrameTimeSpan = TimeSpan.Zero;
        sprite.ParentId = self.parentId;
        sprite.Zindex = self.zindex;
        sprite.Parent = parent;
        sprite.RotMovement = self.rotDrag;
        sprite.Drag = self.drag;
        sprite.Stretch = self.stretchAmount;
        sprite.ShowBlink = (BlinkState)self.showBlink;
        sprite.ShowMouth = (MouthState)self.showTalk;
        sprite.Name = self.path.Replace("user://", string.Empty);

        sprite.LayerStates = self.costumeLayers.Trim('[', ']') // Remove brackets
                                 .Split(',')    // Split by comma
                                 .Select(int.Parse) // Convert each part to an integer
                                 .ToList();

        (var scaleImportFactor, var scaleMidOffset) = MigrateBase64(sprite);
        if (parent != null)
        {
            var pos = self.offset;
            if (pos == "Vector2(0, 0)")
            {
                //var anchor = FindAnchorPoint(Parent.Bitmap, new SKPoint(Parent.Position.X, Parent.Position.Y), Bitmap, new SKPoint(Position.X, Position.Y));
                //Anchor = new Vector2(anchor.X, anchor.Y);
                sprite.Anchor = new Vector2(sprite.Position.X + sprite.Bitmap.Width / 2, sprite.Position.Y + sprite.Bitmap.Height / 2);
            }
            else
            {
                string trimmedInput = pos.Replace("Vector2(", "").Replace(")", "");
                string[] parts = trimmedInput.Split(',');

                sprite.Anchor = new Vector2(
                   float.Parse(parts[0].Trim(), CultureInfo.InvariantCulture) * -scaleImportFactor + scaleMidOffset,
                    float.Parse(parts[1].Trim(), CultureInfo.InvariantCulture) * -scaleImportFactor + scaleMidOffset
                );
            }
        }
        foreach (var item in items.ToList())
        {
            if (item.parentId != sprite.Id) continue;
            var child = MigratePngtuberPlus(item, items, sprite);
            items.Remove(item);
            sprite.Children.Add(child);
        }
        return sprite;
    }
    public static (float scaleImportFactor, float scaleMidOffset) MigrateBase64(SpriteImage sprite)
    {
        byte[] imageBytes = Convert.FromBase64String(sprite.ImageBase64.First());

        using var memoryStream = new MemoryStream(imageBytes);
        using var skStream = new SKManagedStream(memoryStream);
        using var bitmap = SKBitmap.Decode(skStream);

        using var scaled = Resize(bitmap, (int)(Specsmanager.Width * Specsmanager.ScaleFactor), (int)(Specsmanager.Height * Specsmanager.ScaleFactor));
        var scaleImportFactor = (float)scaled.Width / bitmap.Width;
        var scaleMidOffset = scaleImportFactor * bitmap.Width / 2;
        var cropped = CropAndGetOffset(scaled);
        sprite.Bitmap = new StaticImage(cropped.croppedBitmap);
        
        sprite.Position = new Vector2(cropped.offset.X, cropped.offset.Y);

        // save back to base 64
        using var imageData = cropped.croppedBitmap.Encode(SKEncodedImageFormat.Png, 100);
        sprite.ImageBase64 = [Convert.ToBase64String(imageData.ToArray())];
        return (scaleImportFactor, scaleMidOffset);
    }
    public static (SKBitmap croppedBitmap, SKPoint offset) CropAndGetOffset(SKBitmap original)
    {
        // Variables to track the bounds of non-transparent pixels
        int minX = original.Width, minY = original.Height, maxX = 0, maxY = 0;

        // Iterate over each pixel to find the non-transparent area
        for (int y = 0; y < original.Height; y++)
        {
            for (int x = 0; x < original.Width; x++)
            {
                SKColor pixelColor = original.GetPixel(x, y);
                if (pixelColor.Alpha > 0) // If alpha is greater than 0 (non-transparent)
                {
                    minX = Math.Min(minX, x);
                    minY = Math.Min(minY, y);
                    maxX = Math.Max(maxX, x);
                    maxY = Math.Max(maxY, y);
                }
            }
        }

        // If no non-transparent pixels were found, return an empty bitmap
        if (minX == original.Width || minY == original.Height)
        {
            return (null, new SKPoint(0, 0));
        }

        // Define the bounds of the cropped image
        var width = maxX - minX + 1;
        var height = maxY - minY + 1;

        // Create the cropped bitmap
        var croppedBitmap = new SKBitmap(width, height);

        // Copy the relevant pixels into the new bitmap
        using var canvas = new SKCanvas(croppedBitmap);
        canvas.DrawBitmap(original, new SKRectI(minX, minY, maxX + 1, maxY + 1), new SKRect(0, 0, width, height));

        // The offset is the position where the cropped image will be placed (minX, minY)
        var offset = new SKPoint(minX, minY);

        return (croppedBitmap, offset);
    }
    public static SKBitmap Resize(SKBitmap bitmap, int maxWidth, int maxHeight)
    {
        float widthRatio = (float)maxWidth / bitmap.Width;
        float heightRatio = (float)maxHeight / bitmap.Height;
        float scaleRatio = Math.Min(widthRatio, heightRatio);

        int newWidth = (int)(bitmap.Width * scaleRatio);
        int newHeight = (int)(bitmap.Height * scaleRatio);

        using (var resizedBitmap = new SKBitmap(newWidth, newHeight))
        using (var canvas = new SKCanvas(resizedBitmap))
        {
            canvas.DrawBitmap(bitmap, new SKRect(0, 0, newWidth, newHeight));
            return resizedBitmap.Copy();
        }
    }
}
