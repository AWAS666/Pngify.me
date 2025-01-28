﻿using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.Json.Serialization;

namespace PngifyMe.Services.CharacterSetup.Advanced;
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
    public List<int> LayerStates { get; set; } = new List<int>(10);
    public int Drag { get; set; }
    public float Stretch { get; set; }

    [JsonIgnore]
    public SpriteImage? Parent { get; set; }
    public float CurrentRotation { get; internal set; }
    public List<SpriteImage> Children { get; set; } = new List<SpriteImage>();

    public void Update(float deltaTime, Vector2 offset)
    {
        // Calculate the velocity based on the offset difference
        Vector2 velocity = (offset - lastOffset) / deltaTime;

        // Apply drag using an easing function
        if (Drag > 0)
        {
            // Normalize the drag effect (range [0, 1])
            float dragProgress = Math.Clamp(Drag * deltaTime, 0f, 1f);

            // Apply an easing function to the drag progress
            float easedDrag = Easings.QuadraticEaseOut(dragProgress);

            // Reduce the velocity based on the eased drag
            velocity *= (1 - easedDrag);
            }

        // Update the offset based on the smoothed velocity
        Offset = lastOffset + velocity * deltaTime;

        // Calculate the rotation change based on the velocity
        float targetRotation = velocity.Length() * RotMovement * 3;

        // Smoothly interpolate the current rotation towards the target rotation
        float rotationSmoothingFactor = 0.1f; // Adjust this value for smoother or sharper transitions
        CurrentRotation = Easings.Lerp(CurrentRotation, targetRotation, rotationSmoothingFactor);

        // Update all child elements
        foreach (var child in Children)
    {
            child.Update(deltaTime, Offset);
        }

        // Store the current offset for the next frame
        lastOffset = Offset;
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
    Closed = 1,
    Open = 2,
}

public enum MouthState
{
    Ignore = 0,
    Closed = 1,
    Open = 2,
}
