using PngifyMe.Helpers;
using PngifyMe.Layers;
using Serilog;
using SkiaSharp;
using Spout.Interop;
using System;
using System.Threading.Tasks;

namespace PngifyMe.Services
{
    public static class SpoutRenderer
    {
        private static SpoutSender spoutSender;
        private static byte[] pixelData;
        private static byte[] swappedPixels;

        public static void Init()
        {
            try
            {
                spoutSender = new SpoutSender();
                spoutSender.CreateSender("Pngify.me", (uint)Specsmanager.Width, (uint)Specsmanager.Height, 0);
                LayerManager.ImageUpdate += NewImage;
            }
            catch (Exception e)
            {

                throw new Exception($"Spout init error: {e.Message}",e);
            }
          
        }

        private unsafe static void NewImage(object? sender, SaveDispose<SKBitmap> e)
        {
            if (SettingsManager.Current.General.EnableSpout != true)
                return;

            _ = Task.Run(() =>
            {
                try
                {
                    ConvertSKImageToRawByteArray(LayerManager.CurrentFrame.Value);
                    //SwapChannels();
                    // Send the byte array via Spout
                    fixed (byte* pData = pixelData)
                    {
                        // gl.rgba == 6408
                        spoutSender.SendImage(pData, (uint)Specsmanager.Width, (uint)Specsmanager.Height, 6408, false, 0);
                    }
                }
                catch (Exception er)
                {
                    Log.Error(er, "Error in spout send: " + er.Message);
                }
            });
        }

        private unsafe static void SwapChannels()
        {
            if (swappedPixels == null)
                swappedPixels = new byte[pixelData.Length];

            fixed (byte* pSrc = pixelData)
            fixed (byte* pDst = swappedPixels)
            {
                byte* src = pSrc;
                byte* dst = pDst;

                int length = pixelData.Length;
                for (int i = 0; i < length; i += 4)
                {
                    dst[i] = src[i + 2];     // R B
                    dst[i + 1] = src[i + 1]; // G G
                    dst[i + 2] = src[i + 0]; // B R 
                    dst[i + 3] = src[i + 3]; // A A
                }
            }
        }

        public static unsafe void ConvertSKImageToRawByteArray(SKBitmap bitmap)
        {
            // Ensure the bitmap has the correct configuration
            SKImageInfo imageInfo = bitmap.Info;
            imageInfo.ColorType = SKColorType.Rgba8888;

            // Allocate a byte array large enough to hold all the pixel data
            pixelData ??= new byte[imageInfo.BytesSize];

            // Copy the raw pixel data into the byte array
            fixed (byte* ptr = pixelData)
            {
                // Access the pixels directly
                using var pixmap = bitmap.PeekPixels();
                // Read pixel data into the byte array
                pixmap.ReadPixels(imageInfo, (IntPtr)ptr, imageInfo.RowBytes, 0, 0);
            }
        }
    }
}
