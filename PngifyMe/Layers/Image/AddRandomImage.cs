using PngifyMe.Layers.Helper;
using PngifyMe.Services.CharacterSetup.Images;
using Serilog;
using SkiaSharp;
using System;
using System.IO;
using System.Linq;

namespace PngifyMe.Layers.Image;

[LayerDescription("AddRandomImage")]
public class AddRandomImage : ImageLayer
{
    [Unit("pixels (center)")]
    public float PosX { get; set; } = 960;

    [Unit("pixels (center)")]
    public float PosY { get; set; } = 540;

    [Unit("Path")]
    [FolderPicker]
    public string Folder { get; set; } = string.Empty;

    [Unit("seconds")]
    public float StickyFor { get; set; } = float.MaxValue;

    private BaseImage image;

    public override void OnEnter()
    {
        if (!Directory.Exists(Folder))
        {
            Log.Error($"AddRandomImage: Directory {Folder} not found");
            IsExiting = true;
            return;
        }

        var files = Directory.GetFiles(Folder).Where(x => x.EndsWith(".png") || x.EndsWith(".gif"));

        if (files.Count() < 1)
        {
            Log.Error($"AddRandomImage: no files found in folder: {Folder}");
            IsExiting = true;
            return;
        }

        AutoRemoveTime = StickyFor;
        image = BaseImage.LoadFromPath(files.ElementAt(Random.Shared.Next(0, files.Count())));
        base.OnEnter();
    }

    public override void OnExit()
    {
        image?.Dispose();
        base.OnExit();
    }

    public override void OnCalculateParameters(float dt, ref LayerValues values)
    {
        // nothing prolly
    }

    public override void RenderImage(SKCanvas canvas, float x, float y)
    {
        if (IsExiting || image == null)
            return;
        var img = GetImage();
        canvas.DrawBitmap(img, PosX - img.Width / 2, PosY - img.Height / 2);
    }

    public SKBitmap GetImage()
    {
        return image.GetImage(TimeSpan.FromSeconds(CurrentTime));
    }
}
