using ImageMagick;
using SkiaSharp;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Text.Json;

public class Program
{
    private static int layer = 0;
    private static MouthState mouthState = MouthState.Closed;
    public static async Task Main(string[] args)
    {
        SpriteImage spriteParent = await LoadAndConvertPngtuberPlus();

        int canvasWidth = 1920;
        int canvasHeight = 1080;

        float deltaTime = 1f / 60;
        var all = spriteParent.GetAllSprites().OrderBy(x => x.Zindex).ToList();

        RenderAllItems(all, canvasWidth, canvasHeight);

        using var collection = new MagickImageCollection();
        float curTime = 0f;
        for (int i = 0; i < 360; i++)
        {
            spriteParent.Update(deltaTime, new Vector2(0, 15 * (float)Math.Sin(curTime / 18 * 180 / Math.PI)));
            using var surface = Render(all, canvasWidth, canvasHeight);
            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            //using var fileStream = File.OpenWrite($"output{i}.png");
            //data.SaveTo(fileStream);

            // Create a MagickImage from the byte array
            var bytes = data.ToArray();
            var img = new MagickImage(bytes);
            img.AnimationDelay = 2;
            img.GifDisposeMethod = GifDisposeMethod.Previous;
            collection.Add(img);
            curTime += deltaTime;

            if (i == 180)
                mouthState = MouthState.Open;
            if (i == 300)
                mouthState = MouthState.Closed;
        }

        collection[0].AnimationIterations = 0;

        // Write to output file
        collection.Write("test.gif");

        await File.WriteAllTextAsync("migrated.psave", JsonSerializer.Serialize(spriteParent));

        Console.WriteLine("Image saved as output.png");

    }

    private static async Task<SpriteImage> LoadAndConvertPngtuberPlus()
    {
        var file = await File.ReadAllTextAsync("mocha.save");
        var obj = JsonSerializer.Deserialize<Dictionary<string, PngTuberPlusObject>>(file);
        var items = obj.Values.ToList();
        var parent = items.First(x => x.parentId == null);
        items.Remove(parent);

        var spriteParent = new SpriteImage();
        spriteParent.MigratePngtuberPlus(parent, items);
        return spriteParent;
    }

    private static async Task<SpriteImage> LoadConfig()
    {
        var file = await File.ReadAllTextAsync("migrated.psave");
        var spriteParent = JsonSerializer.Deserialize<SpriteImage>(file);
        spriteParent.Load();
        return spriteParent;
    }

    private static void RenderAllItems(List<SpriteImage> items, int canvasWidth, int canvasHeight)
    {
        using SKPaint paint = new SKPaint();
        paint.Color = SKColors.Blue;
        paint.IsAntialias = true;
        foreach (var item in items)
        {
            var surface = SKSurface.Create(new SKImageInfo(canvasWidth, canvasHeight));
            var canvas = surface.Canvas;
            // Save the current canvas state
            canvas.Save();

            // Apply transformations
            canvas.RotateDegrees(item.CurrentRotation, item.Anchor.X, item.Anchor.Y);

            // Draw the rotated bitmap
            //canvas.DrawBitmap(item.Bitmap, 0, 0);
            canvas.DrawBitmap(item.Bitmap, item.CurrentPosition.X, item.CurrentPosition.Y);
            // Restore the canvas to the original state
            canvas.Restore();

            // Draw the red dot
            canvas.DrawCircle(item.Anchor.X + item.Offset.X, item.Anchor.Y + item.Offset.Y, 5, paint);

            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var fileStream = File.OpenWrite(item.Name.Replace("/", "_"));
            data.SaveTo(fileStream);
        }
    }

    private static SKSurface Render(List<SpriteImage> items, int canvasWidth, int canvasHeight)
    {
        var surface = SKSurface.Create(new SKImageInfo(canvasWidth, canvasHeight));
        var canvas = surface.Canvas;

        var watch = new Stopwatch();
        watch.Start();
        using SKPaint paint = new SKPaint();
        paint.Color = SKColors.Red;
        paint.IsAntialias = true;
        float radius = 5; // Radius of the dot
        var rel = items
            .Where(x => x.LayerStates[layer] == 1)
            .Where(x => x.ShowMouth == MouthState.Ignore || x.ShowMouth == mouthState);
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

            // Draw the red dot
            canvas.DrawCircle(item.Anchor.X + item.Offset.X, item.Anchor.Y + item.Offset.Y, radius, paint);
        }
        Console.WriteLine($"Rendered {rel.Count()} in {watch.ElapsedMilliseconds}ms {watch.ElapsedTicks}ticks");
        watch.Restart();

        return surface;
    }


    //private static SKSurface V1(List<PngTuberPlusObject> items, int canvasWidth, int canvasHeight)
    //{
    //    var surface = SKSurface.Create(new SKImageInfo(canvasWidth, canvasHeight));
    //    var canvas = surface.Canvas;

    //    int index = 1;
    //    var bitmaps = new List<SKBitmap>();
    //    foreach (var img in items.OrderBy(x => x.zindex))
    //    {
    //        byte[] imageBytes = Convert.FromBase64String(img.imageData);

    //        using var memoryStream = new MemoryStream(imageBytes);
    //        using var skStream = new SKManagedStream(memoryStream);
    //        using var bitmap = SKBitmap.Decode(skStream);
    //        var scaled = Resize(bitmap, (int)(canvasWidth * 0.9f), (int)(canvasHeight * 0.9f));

    //        //using var skimg = SKImage.FromBitmap(scaled);
    //        //using var dat = skimg.Encode(SKEncodedImageFormat.Png, 100);
    //        //var fileName = $"image_{index++}.png";
    //        //using var fs = File.OpenWrite(fileName);
    //        //dat.SaveTo(fs);

    //        bitmaps.Add(scaled);
    //    }

    //    var watch = new Stopwatch();
    //    watch.Start();
    //    for (int i = 0; i < 10; i++)
    //    {
    //        foreach (var bitmap in bitmaps)
    //        {
    //            // Draw the bitmap on the canvas
    //            canvas.DrawBitmap(bitmap, 0, 0);
    //        }
    //        Debug.WriteLine($"Rendered {bitmaps.Count} in {watch.ElapsedMilliseconds}ms {watch.ElapsedTicks}ticks");
    //        watch.Restart();
    //    }

    //    return surface;
    //}

    //private static SKSurface V2(List<PngTuberPlusObject> items, int canvasWidth, int canvasHeight)
    //{
    //    var surface = SKSurface.Create(new SKImageInfo(canvasWidth, canvasHeight));
    //    var canvas = surface.Canvas;

    //    var bitmaps = new List<SKBitmap>();
    //    foreach (var img in items.OrderBy(x => x.zindex))
    //    {
    //        byte[] imageBytes = Convert.FromBase64String(img.imageData);

    //        using var memoryStream = new MemoryStream(imageBytes);
    //        using var skStream = new SKManagedStream(memoryStream);
    //        using var bitmap = SKBitmap.Decode(skStream);
    //        var scaled = Resize(bitmap, (int)(canvasWidth * 0.9f), (int)(canvasHeight * 0.9f));
    //        scaled.SetImmutable();
    //        bitmaps.Add(scaled);
    //    }

    //    var watch = new Stopwatch();
    //    watch.Start();
    //    for (int i = 0; i < 10; i++)
    //    {
    //        foreach (var bitmap in bitmaps)
    //        {
    //            // Draw the bitmap on the canvas
    //            canvas.DrawBitmap(bitmap, 0, 0);
    //        }
    //        Debug.WriteLine($"Rendered {bitmaps.Count} in {watch.ElapsedMilliseconds}ms {watch.ElapsedTicks}ticks");
    //        watch.Restart();
    //    }

    //    return surface;
    //}

    //private static SKSurface V3(List<PngTuberPlusObject> items, int canvasWidth, int canvasHeight)
    //{
    //    var surface = SKSurface.Create(new SKImageInfo(canvasWidth, canvasHeight));
    //    var canvas = surface.Canvas;

    //    var bitmaps = new List<(SKBitmap, SKPoint)>();
    //    foreach (var img in items.OrderBy(x => x.zindex))
    //    {
    //        byte[] imageBytes = Convert.FromBase64String(img.imageData);

    //        using var memoryStream = new MemoryStream(imageBytes);
    //        using var skStream = new SKManagedStream(memoryStream);
    //        using var bitmap = SKBitmap.Decode(skStream);
    //        using var scaled = Resize(bitmap, (int)(canvasWidth * 0.9f), (int)(canvasHeight * 0.9f));

    //        //https://github.com/mono/SkiaSharp/issues/2188 -> this is a big performance improvment
    //        //scaled.SetImmutable();
    //        var cropped = CropAndGetOffset(scaled);
    //        cropped.croppedBitmap.SetImmutable();

    //        bitmaps.Add(cropped);
    //        //using var skimg = SKImage.FromBitmap(bitmap);
    //        //using var dat = skimg.Encode(SKEncodedImageFormat.Png, 100);
    //        //var fileName = $"image_{index++}.png";
    //        //using var fs = File.OpenWrite(fileName);
    //        //dat.SaveTo(fs);
    //    }


    //    var watch = new Stopwatch();
    //    watch.Start();
    //    for (int i = 0; i < 10; i++)
    //    {
    //        foreach (var bitmap in bitmaps)
    //        {
    //            // Draw the bitmap on the canvas
    //            canvas.DrawBitmap(bitmap.Item1, bitmap.Item2.X, bitmap.Item2.Y);
    //        }
    //        Debug.WriteLine($"Rendered {bitmaps.Count} in {watch.ElapsedMilliseconds}ms {watch.ElapsedTicks}ticks");
    //        watch.Restart();
    //    }

    //    return surface;
    //}




}