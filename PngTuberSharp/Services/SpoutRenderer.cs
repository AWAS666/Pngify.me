using OpenGL;
using PngTuberSharp.Layers;
using SkiaSharp;
using Spout.Interop;
using System;
using System.Threading;

namespace PngTuberSharp.Services
{
    public static class SpoutRenderer
    {
        private static SpoutSender spoutSender;

        static SpoutRenderer()
        {
            LayerManager.ImageUpdate += NewImage;
        }

        internal static void Init()
        {

        }

        private unsafe static void NewImage(object? sender, SKImage e)
        {
            try
            {
                byte[] imageBytes = ConvertSKImageToRawByteArray(e);
                spoutSender = new SpoutSender();
                // Send the byte array via Spout
                fixed (byte* pData = imageBytes)
                {
                    spoutSender.SendImage(pData, 1920, 1080, Gl.RGBA, false, 0);
                }
            }
            catch (Exception er)
            {
            }
        }       

        public static unsafe byte[] ConvertSKImageToRawByteArray(SKImage image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            // Create a bitmap from the image
            using (SKBitmap bitmap = SKBitmap.FromImage(image))
            {
                // Ensure the bitmap has the correct configuration
                SKImageInfo imageInfo = bitmap.Info;

                // Allocate a byte array large enough to hold all the pixel data
                byte[] pixelData = new byte[imageInfo.BytesSize];

                // Copy the raw pixel data into the byte array
                fixed (byte* ptr = pixelData)
                {
                    // Access the pixels directly
                    using (SKPixmap pixmap = bitmap.PeekPixels())
                    {
                        // Read pixel data into the byte array
                        pixmap.ReadPixels(imageInfo, (IntPtr)ptr, imageInfo.RowBytes, 0, 0);
                    }
                }

                return pixelData; // Return the raw pixel data as a byte array
            }
        }

        private static unsafe void SendSpout()
        {
            using (var deviceContext = DeviceContext.Create())
            {
                IntPtr glContext = deviceContext.CreateContext(IntPtr.Zero);
                deviceContext.MakeCurrent(glContext);

                spoutSender = new SpoutSender();

                while (true)
                {
                    try
                    {
                        // Render the control to a byte array
                        byte[] data = new byte[2048];

                        // Send the byte array via Spout
                        fixed (byte* pData = data)
                        {
                            spoutSender.SendImage(pData, 1920, 1080, Gl.RGBA, false, 0);
                        }

                        // Simulate 60 FPS (adjust as necessary)
                        Thread.Sleep(1000 / 30); // ~60 FPS
                    }
                    catch (Exception e)
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
        }
    }
}
