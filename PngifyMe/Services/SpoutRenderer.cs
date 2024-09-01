using OpenGL;
using PngifyMe.Layers;
using Serilog;
using SkiaSharp;
using Spout.Interop;
using System;
using System.Linq;
using System.Threading;

namespace PngifyMe.Services
{
    public static class SpoutRenderer
    {
        private static SpoutSender spoutSender;
        private static byte[] pixelData;
        private static byte[] swappedPixels;

        public static void Init()
        {
            spoutSender = new SpoutSender();
            LayerManager.ImageUpdate += NewImage;
        }

        private unsafe static void NewImage(object? sender, SKImage e)
        {
            if (SettingsManager.Current.General.EnableSpout != true)
                return;
            
            try
            {
                //using var deviceContext = DeviceContext.Create();
                //IntPtr glContext = deviceContext.CreateContext(IntPtr.Zero);
                //deviceContext.MakeCurrent(glContext);

                ConvertSKImageToRawByteArray(e);
                SwapChannels();
                // Send the byte array via Spout
                fixed (byte* pData = swappedPixels)
                {
                    spoutSender.SendImage(pData, 1920, 1080, Gl.RGBA, false, 0);
                }
            }
            catch (Exception er)
            {
                Log.Error(er, "Error in spout send: " + er.Message);
            }
        }

        private static void SwapChannels()
        {
            if (swappedPixels == null)
                swappedPixels = new byte[pixelData.Count()];
            for (int i = 0; i < pixelData.Length; i += 4)
            {
                swappedPixels[i] = pixelData[i + 2];     // R
                swappedPixels[i + 1] = pixelData[i + 1]; // G
                swappedPixels[i + 2] = pixelData[i + 0]; // B
                swappedPixels[i + 3] = pixelData[i + 3];     // A
            }
        }

        public static unsafe void ConvertSKImageToRawByteArray(SKImage image)
        {
            // Create a bitmap from the image
            using var bitmap = SKBitmap.FromImage(image);
            // Ensure the bitmap has the correct configuration
            SKImageInfo imageInfo = bitmap.Info;

            // Allocate a byte array large enough to hold all the pixel data
            if (pixelData == null)
                pixelData = new byte[imageInfo.BytesSize];

            // Copy the raw pixel data into the byte array
            fixed (byte* ptr = pixelData)
            {
                // Access the pixels directly
                using var pixmap = bitmap.PeekPixels();
                // Read pixel data into the byte array
                pixmap.ReadPixels(imageInfo, (IntPtr)ptr, imageInfo.RowBytes, 0, 0);
            }

            //return pixelData; // Return the raw pixel data as a byte array
        }
    }
}
