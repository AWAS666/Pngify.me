using Avalonia.Media.Imaging;
using PngTuberSharp.Layers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PngTuberSharp.Services.ThrowingSystem
{
    public class MovableObject
    {
        private LayerValues Values;

        private Vector2 speed;

        public Vector2 CurrentSpeed => speed;
        public List<(int x, int y)> Outline { get; }
        public float X { get => Values.PosX; }
        public float Y { get => Values.PosY; }

        public MovableObject(string path, Vector2 speed)
        {
            Values = new LayerValues();
            Values.Image = new Bitmap(path);
            Outline = GetImageOutline();
            this.speed = speed;
        }

        public MovableObject(Bitmap map, Vector2 speed)
        {
            Values = new LayerValues();
            Values.Image = map;
            Outline = GetImageOutline();
            this.speed = speed;
        }

        public void Update(float dt)
        {
            Values.PosX += speed.X * dt;
            Values.PosY += speed.Y * dt;
        }

        public void SetCollision()
        {
            // just invert vector I guess
            speed.X = -speed.X;
            speed.Y = -speed.Y;

            // update position to make it move away quicker
            Update(0.1f);
        }

        private List<(int x, int y)> GetImageOutline()
        {
            var outline = new List<(int x, int y)>();

            using var ms = new MemoryStream();
            Values.Image.Save(ms);

            var bitmap = new System.Drawing.Bitmap(ms);

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    var color = bitmap.GetPixel(x, y);
                    if (color.A > 0) // If pixel is not fully transparent
                    {
                        outline.Add((x, y));
                    }
                }
            }

            return outline;
        }

        public bool SameBitmap(Bitmap bitmap)
        {
            return bitmap == Values.Image;
        }
    }
}
