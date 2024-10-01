using PngifyMe.Layers.Helper;
using PngifyMe.Services;
using PngifyMe.Services.Settings.Images;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PngifyMe.Layers.Image;

[LayerDescription("Explode your png")]
public class Explosion : ImageLayer
{

    [Unit("seconds")]
    public float RecoverIn { get; set; } = 5f;

    [Unit("multiplier")]
    public float SpeedFactor { get; set; } = 50f;

    private List<Particle> particles;
    private BaseImage frame;
    private bool drawing;
    private float firstframe;

    public override void OnEnter()
    {
        AutoRemoveTime = RecoverIn;
        ApplyOtherEffects = true;
        EnterTime = 0f;
        ExitTime = 1f;
        // save current frame as the one to explode
        frame = LayerManager.MicroPhoneStateLayer.CurrentImage;

        // Number of particles for the explosion effect
        int particleSize = 40; // Size of each particle

        // Create the particles
        particles = CreateParticles(frame.Preview, particleSize);
        base.OnEnter();
    }

    public override void OnExit()
    {
        foreach (var particle in particles)
        {
            particle.Dispose();
        }
        base.OnExit();
    }

    public override void OnCalculateParameters(float dt, ref LayerValues values)
    {
        // hide original
        if (drawing)
            values.Opacity = 1 - CurrentStrength;
    }

    public override void RenderImage(SKCanvas canvas, float x, float y)
    {
        if (IsExiting || particles == null)
            return;
        if (!drawing)
        {
            drawing = true;
            firstframe = CurrentTime;
        }
        float width = Specsmanager.Width;
        float height = Specsmanager.Height;
        foreach (var particle in particles)
        {
            particle.Update((CurrentTime - firstframe) * SpeedFactor, x + width / 2 - frame.Preview.Width / 2, y + height / 2 - frame.Preview.Height / 2);
            particle.Draw(canvas);
        }
    }

    public override void OnUpdate(float dt, float time)
    {
        CurrentStrength = 1.0f;
        base.OnUpdate(dt, time);
    }

    public override void OnUpdateEnter(float dt, float fraction)
    {
        CurrentStrength = Easings.CubicEaseOut(fraction);
        base.OnUpdateEnter(dt, fraction);
    }

    public override void OnUpdateExit(float dt, float fraction)
    {
        CurrentStrength = 1.0f - Easings.CubicEaseOut(fraction);
        base.OnUpdateExit(dt, fraction);
    }

    static List<Particle> CreateParticles(SKBitmap image, int particleSize)
    {
        var particles = new List<Particle>();

        // Calculate the center of the image
        var centerX = image.Width / 2;
        var centerY = image.Height / 2;

        // Lock object to safely add particles from multiple threads
        var lockObj = new object();

        // Multithreaded loop using Parallel.For
        Parallel.For(0, image.Height / particleSize, yIndex =>
        {
            for (int x = 0; x < image.Width; x += particleSize)
            {
                int y = yIndex * particleSize;

                var particleRect = new SKRectI(x, y, x + particleSize, y + particleSize);
                var particleBitmap = new SKBitmap(particleSize, particleSize);
                using (var canvas = new SKCanvas(particleBitmap))
                {
                    canvas.DrawBitmap(image, particleRect, new SKRect(0, 0, particleSize, particleSize));
                }

                var particle = new Particle(particleBitmap, new SKPoint(x, y), new SKPoint(centerX, centerY));

                // Lock to safely add the particle to the list
                lock (lockObj)
                {
                    particles.Add(particle);
                }
            }
        });


        return particles;
    }
}
class Particle : IDisposable
{
    private SKBitmap bitmap;
    private SKPoint originalPosition;
    private SKPoint currentPosition;
    private SKPoint velocity;
    private SKPoint center;

    public Particle(SKBitmap bitmap, SKPoint position, SKPoint center)
    {
        this.bitmap = bitmap;
        this.originalPosition = position;
        this.currentPosition = position;
        this.center = center;

        // Calculate direction from the center point to the particle's initial position
        var directionX = position.X - center.X;
        var directionY = position.Y - center.Y;

        // Normalize the direction and apply a velocity multiplier
        var magnitude = (float)Math.Sqrt(directionX * directionX + directionY * directionY);
        if (magnitude > 0)
        {
            directionX /= magnitude;
            directionY /= magnitude;
        }

        // Adjust the velocity to control how fast the particles move
        float velocityMultiplier = 5.0f;
        this.velocity = new SKPoint(directionX * velocityMultiplier, directionY * velocityMultiplier);
    }

    public void Update(float dt, float x, float y)
    {
        // Move the particle based on velocity
        currentPosition.X = originalPosition.X + velocity.X * dt + x;
        currentPosition.Y = originalPosition.Y + velocity.Y * dt + y;
    }

    public void Draw(SKCanvas canvas)
    {
        canvas.DrawBitmap(bitmap, currentPosition);
    }

    public void Dispose()
    {
        bitmap?.Dispose();
    }
}
