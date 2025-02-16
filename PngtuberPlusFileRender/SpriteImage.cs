using SkiaSharp;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;
using System.Text.Json.Serialization;

public class SpriteImage
{
    private Vector2 lastOffset = Vector2.Zero;
    private float scaleImportFactor;
    private float scaleMidOffset;

    /// <summary>
    /// todo:
    /// refactor resize and crop code to a common class as this is also used elsewhere
    /// scaling, always scale to 1920x1080 when migrating?
    /// </summary>
    public long Id { get; set; }
    public string Name { get; set; }
    public string ImageBase64 { get; set; }

    [JsonIgnore]
    public SKBitmap Bitmap { get; set; }
    public long? ParentId { get; set; }

    [JsonIgnore]
    public Vector2 Offset { get; set; } = Vector2.Zero;

    [JsonIgnore]
    public Vector2 CurrentPosition => Position + Offset;

    [JsonIgnore]
    public Vector2 Position { get; set; }

    /// <summary>
    /// used for json serialize
    /// </summary>
    public float[] PositionArray
    {
        get => [Position.X, Position.Y];
        set => Position = new Vector2(value[0], value[1]);
    }

    [JsonIgnore]
    public Vector2 Anchor { get; set; }

    /// <summary>
    /// used for json serialize
    /// </summary>
    public float[] AnchorArray
    {
        get => [Anchor.X, Anchor.Y];
        set => Anchor = new Vector2(value[0], value[1]);
    }

    public int Zindex { get; set; }

    public int RotMovement { get; set; }
    public BlinkState ShowBlink { get; set; }
    public MouthState ShowMouth { get; set; }
    public List<int> LayerStates { get; set; }
    public int Drag { get; set; }
    public float Stretch { get; set; }

    [JsonIgnore]
    public SpriteImage? Parent { get; set; }
    public float CurrentRotation { get; internal set; }
    public List<SpriteImage> Children { get; set; } = new List<SpriteImage>();

    public void Update(float deltaTime, Vector2 offset)
    {
        if (Parent != null)
        {
            if (Drag == 0)
                Offset = offset;
            else
            {
                Offset = offset * (0.5f / Drag + 0.5f);
                Offset = (1 * Parent.Offset + 3 * Offset) / 4;
            }
        }
        else
            Offset = offset;

        CurrentRotation = (Offset - lastOffset).Length() * RotMovement * 3;

        foreach (var child in Children)
        {
            child.Update(deltaTime, Offset);
        }
        lastOffset = Offset;
    }

    public void MigratePngtuberPlus(PngTuberPlusObject self, List<PngTuberPlusObject> items, SpriteImage parent = null)
    {
        Id = self.identification;
        ImageBase64 = self.imageData;
        ParentId = self.parentId;
        Zindex = self.zindex;
        Parent = parent;
        RotMovement = self.rotDrag;
        Drag = self.drag;
        Stretch = self.stretchAmount;
        ShowBlink = (BlinkState)self.showBlink;
        ShowMouth = (MouthState)self.showTalk;
        Name = self.path.Replace("user://", string.Empty);

        LayerStates = self.costumeLayers.Trim('[', ']') // Remove brackets
                                 .Split(',')    // Split by comma
                                 .Select(int.Parse) // Convert each part to an integer
                                 .ToList();
        MigrateBase64();
        if (parent != null)
        {
            var pos = self.offset;
            if (pos == "Vector2(0, 0)")
            {
                //var anchor = FindAnchorPoint(Parent.Bitmap, new SKPoint(Parent.Position.X, Parent.Position.Y), Bitmap, new SKPoint(Position.X, Position.Y));
                //Anchor = new Vector2(anchor.X, anchor.Y);
                Anchor = new Vector2(Position.X + Bitmap.Width / 2, Position.Y + Bitmap.Height / 2);
            }
            else
            {
                string trimmedInput = pos.Replace("Vector2(", "").Replace(")", "");
                string[] parts = trimmedInput.Split(',');

                Anchor = new Vector2(
                   float.Parse(parts[0].Trim(), CultureInfo.InvariantCulture) * -scaleImportFactor + scaleMidOffset,
                    float.Parse(parts[1].Trim(), CultureInfo.InvariantCulture) * -scaleImportFactor + scaleMidOffset
                );
            }
        }
        foreach (var item in items.ToList())
        {
            if (item.parentId != Id) continue;
            var child = new SpriteImage();
            items.Remove(item);
            child.MigratePngtuberPlus(item, items, this);
            Children.Add(child);
        }
    }
    private void MigrateBase64()
    {
        byte[] imageBytes = Convert.FromBase64String(ImageBase64);

        using var memoryStream = new MemoryStream(imageBytes);
        using var skStream = new SKManagedStream(memoryStream);
        using var bitmap = SKBitmap.Decode(skStream);

        using var scaled = Resize(bitmap, (int)(1920 * 0.9f), (int)(1080 * 0.9f));
        scaleImportFactor = (float)scaled.Width / bitmap.Width;
        scaleMidOffset = scaleImportFactor * bitmap.Width / 2;
        var cropped = CropAndGetOffset(scaled);
        Bitmap = cropped.croppedBitmap;
        //https://github.com/mono/SkiaSharp/issues/2188 -> this is a big performance improvment
        Bitmap.SetImmutable();
        Position = new Vector2(cropped.offset.X, cropped.offset.Y);

        // save back to base 64
        using var imageData = Bitmap.Encode(SKEncodedImageFormat.Png, 100);
        ImageBase64 = Convert.ToBase64String(imageData.ToArray());
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
    public List<SpriteImage> GetAllSprites()
    {
        var sprites = new List<SpriteImage>
        {
            this
        };
        foreach (var child in Children)
        {
            sprites.AddRange(child.GetAllSprites());
        }
        return sprites;
    }
    public void Load(SpriteImage parent = null)
    {
        byte[] imageBytes = Convert.FromBase64String(ImageBase64);
        using var memoryStream = new MemoryStream(imageBytes);
        using var skStream = new SKManagedStream(memoryStream);
        Bitmap = SKBitmap.Decode(skStream);

        Parent = parent;

        foreach (var child in Children)
        {
            child.Load(this);
        }
    }
    public SKPoint FindAnchorPoint(SKBitmap parentBitmap, SKPoint parentPosition, SKBitmap childBitmap, SKPoint childPosition)
    {
        // Get the bounding boxes of both bitmaps
        var parentRect = new SKRect(parentPosition.X, parentPosition.Y, parentPosition.X + parentBitmap.Width, parentPosition.Y + parentBitmap.Height);
        var childRect = new SKRect(childPosition.X, childPosition.Y, childPosition.X + childBitmap.Width, childPosition.Y + childBitmap.Height);

        // Calculate the intersection of both bitmaps (bounding boxes)
        var intersectRect = SKRect.Intersect(parentRect, childRect);

        // If the intersection area is valid (i.e., they overlap)
        if (!intersectRect.IsEmpty)
        {
            // Find the center of the intersection area (middle of the cross section)
            float centerX = (intersectRect.Left + intersectRect.Right) / 2;
            float centerY = (intersectRect.Top + intersectRect.Bottom) / 2;
            return new SKPoint(centerX, centerY);
        }
        else
        {
            // No overlap: Calculate the closest point between the two bitmaps
            float closestX = (parentPosition.X + parentBitmap.Width / 2);
            float closestY = (parentPosition.Y + parentBitmap.Height / 2);

            // Check if the child is to the left or right of the parent
            if (childPosition.X > parentPosition.X + parentBitmap.Width)
                closestX = parentPosition.X + parentBitmap.Width;
            else if (childPosition.X + childBitmap.Width < parentPosition.X)
                closestX = parentPosition.X;

            // Check if the child is above or below the parent
            if (childPosition.Y > parentPosition.Y + parentBitmap.Height)
                closestY = parentPosition.Y + parentBitmap.Height;
            else if (childPosition.Y + childBitmap.Height < parentPosition.Y)
                closestY = parentPosition.Y;

            return new SKPoint(closestX, closestY);
        }
    }
}

public enum BlinkState
{
    Ignore = 0,
    Open = 1,
    Closed = 2,
}

public enum MouthState
{
    Ignore = 0,
    Open = 1,
    Closed = 2,
}
