using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PngifyMe.Layers;
using PngifyMe.Layers.Helper;
using PngifyMe.Services.CharacterSetup.Images;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.Json.Serialization;

namespace PngifyMe.Services.CharacterSetup.Advanced;
public partial class SpriteImage : ObservableObject
{
    public static List<BlinkState> BlinkStates { get; }
    public static List<MouthState> MouthStates { get; }

    static SpriteImage()
    {
        BlinkStates = new List<BlinkState>(Enum.GetValues(typeof(BlinkState)).Cast<BlinkState>());
        MouthStates = new List<MouthState>(Enum.GetValues(typeof(MouthState)).Cast<MouthState>());
    }

    private Vector2 lastOffset = Vector2.Zero;
    public long Id { get; set; }

    [ObservableProperty]
    private string name;

    /// <summary>
    /// this can be multiple if gif
    /// </summary>
    public List<string> ImageBase64 { get; set; } = new();

    /// <summary>
    /// frame time span
    /// might have to set these for each frame, but most gifs don't do that....
    /// </summary>
    public TimeSpan FrameTimeSpan { get; set; }

    [JsonIgnore]
    public BaseImage Bitmap { get; set; }
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

    [ObservableProperty]
    private int rotMovement;
    public BlinkState ShowBlink { get; set; }
    public MouthState ShowMouth { get; set; }
    public List<int> LayerStates { get; set; } = new List<int>(10);

    [ObservableProperty]
    private int drag;
    public float Stretch { get; set; }

    [property: JsonIgnore]
    [ObservableProperty]
    private SpriteImage? parent;

    [JsonIgnore]
    public float CurrentRotation { get; internal set; }
    public ObservableCollection<SpriteImage> Children { get; set; } = new();

    public void Update(float deltaTime, Vector2 offset)
    {
        // Calculate the velocity based on the offset difference
        Vector2 velocity = offset - lastOffset;

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
        Offset = lastOffset + velocity;

        // Calculate the rotation change based on the velocity
        float targetRotation = Math.Min(velocity.Length(), 1) * RotMovement;

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
        if (ImageBase64.Count == 1)
            Bitmap = new StaticImage(ImageBase64.First(), true);
        else
            Bitmap = new GifImage(ImageBase64, FrameTimeSpan);

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

    public void SwitchImage(string path)
    {
        Bitmap = BaseImage.LoadFromPath(path);
        Bitmap.ConvertToBase64();
        ImageBase64 = Bitmap.ConvertToBase64();
        // set duration to first frame
        if (Bitmap is GifImage gif)
            FrameTimeSpan = gif.Frames.First().Duration;
    }

    [RelayCommand]
    public void Remove()
    {
        Parent?.Children.Remove(this);
        ((SpriteCharacterSetup)LayerManager.CharacterStateHandler.CharacterSetup).ReloadLayerList();
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
