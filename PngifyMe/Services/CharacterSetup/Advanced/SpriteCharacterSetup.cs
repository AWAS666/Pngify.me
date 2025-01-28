using Avalonia.Controls;
using Avalonia.Data;
using NAudio.CoreAudioApi;
using PngifyMe.Layers;
using PngifyMe.Services.CharacterSetup.Basic;
using PngifyMe.Services.CharacterSetup.Images;
using Semi.Avalonia.Tokens.Palette;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace PngifyMe.Services.CharacterSetup.Advanced;

public class SpriteCharacterSetup : ICharacterSetup
{
    private SpriteImage parent;
    private List<SpriteImage> layers;

    private static int layer = 0;
    private static MouthState mouthState = MouthState.Closed;
    private static BlinkState blinkState = BlinkState.Open;
    private double transTime;
    private SKBitmap mainBitmap;

    public BaseImage CurrentImage { get; set; } = new StaticImage(ImageSetting.PlaceHolder);

    public IAvatarSettings Settings { get; set; }
    private SpriteCharacterSettings settings => (SpriteCharacterSettings)Settings;

    public float CurrentTime { get; private set; }

    public bool RenderPosition => false;
    public bool RefreshCollisionOnChange => false;

    public void DrawTransition(SKBitmap baseImg, int width, int height, SKCanvas canvas, float opacity)
    {
        // do nothing
    }

    public void RefreshCharacterSettings()
    {
        parent = settings.Parent;
        if (!string.IsNullOrEmpty(parent.ImageBase64))
            parent.Load();
        layers = [.. parent.GetAllSprites().OrderBy(x => x.Zindex)];
    }

    public void SetupHotKeys()
    {
        //todo
    }

    public void ToggleState(string state)
    {
        var newState = settings.States.First(x => x.Name == state);
        layer = newState.Index;
    }

    public void Update(float dt, ref LayerValues values)
    {
        CurrentTime += dt;
        SetBlinkMouth();

        if (parent.Bitmap == null) return;
        int canvasWidth = Specsmanager.Width;
        int canvasHeight = Specsmanager.Height;
        var watch = new Stopwatch();
        watch.Start();

        parent.Update(dt, new Vector2(values.PosX, values.PosY));

        mainBitmap?.Dispose();
        mainBitmap = new SKBitmap(canvasWidth, canvasHeight);
        using var canvas = new SKCanvas(mainBitmap);
        var rel = layers
           .Where(x => x.Bitmap != null)
           .Where(x => x.LayerStates[layer] == 1)
           .Where(x => x.ShowMouth == MouthState.Ignore || x.ShowMouth == mouthState)
           .Where(x => x.ShowBlink == BlinkState.Ignore || x.ShowBlink == blinkState);

        //todo: save rescaled dimension and translate by that
        canvas.Translate((canvasWidth - canvasHeight) / 2, 0);
        foreach (var item in rel)
        {
            // Save the current canvas state
            canvas.Save();

            // Apply transformations
            canvas.RotateDegrees(item.CurrentRotation, item.Anchor.X, item.Anchor.Y);

            // Draw the rotated bitmap
            //canvas.DrawBitmap(item.Bitmap, 0, 0);
            canvas.DrawBitmap(item.Bitmap, item.CurrentPosition.X, item.CurrentPosition.Y);
            // Restore the canvas to the original state
            canvas.Restore();
        }

        Debug.WriteLine($"Rendered {rel.Count()} in {watch.ElapsedMilliseconds}ms {watch.ElapsedTicks}ticks");

        values.Image = mainBitmap;
    }

    private void SetBlinkMouth()
    {
        if (blinkState == BlinkState.Open && CurrentTime > transTime)
        {
            transTime += settings.BlinkTime;
            blinkState = BlinkState.Closed;
        }
        else if (blinkState == BlinkState.Closed && CurrentTime > transTime)
        {
            transTime += settings.BlinkInterval;
            blinkState = BlinkState.Open;
        }

        if (AudioService.Talking)
            mouthState = MouthState.Open;
        else
            mouthState = MouthState.Closed;
    }
}
