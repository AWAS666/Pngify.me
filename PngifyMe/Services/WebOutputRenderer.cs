using PngifyMe.Helpers;
using PngifyMe.Layers;
using Serilog;
using SkiaSharp;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PngifyMe.Services
{
    public static class WebOutputRenderer
    {
        private const int Port = 7667;
        private const int BmpHeaderSize = 54;
        private const byte AlphaThreshold = 128;

        private static HttpListener? _listener;
        private static readonly object _bufferLock = new();
        private static byte[]? _pixelData;
        private static byte[]? _bmpBufferFront;
        private static byte[]? _bmpBufferBack;
        private static int _pixelWidth;
        private static int _pixelHeight;
        private static volatile bool _buildInProgress;

        public static void Init()
        {
            LayerManager.ImageUpdate += OnImageUpdate;

            try
            {
                _listener = new HttpListener();
                _listener.Prefixes.Add($"http://127.0.0.1:{Port}/");
                _listener.Start();
                _ = Task.Run(ListenLoop);
            }
            catch (HttpListenerException ex)
            {
                Log.Warning(ex, "Web output server could not start (port {Port} may be in use)", Port);
                _listener = null;
            }
        }

        private static void OnImageUpdate(object? sender, SaveDispose<SKBitmap> e)
        {
            if (SettingsManager.Current.General.EnableWebOutput != true)
                return;
            if (_buildInProgress)
                return;

            _ = Task.Run(() =>
            {
                try
                {
                    _buildInProgress = true;
                    var frame = LayerManager.CurrentFrame;
                    if (frame?.Value == null || frame.Disposed)
                        return;

                    if (!CopyBitmapToRawByteArray(frame.Value))
                        return;

                    BuildBmpFromRgbaIntoBack();
                    lock (_bufferLock)
                    {
                        (_bmpBufferFront, _bmpBufferBack) = (_bmpBufferBack, _bmpBufferFront);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Web output error: {Message}", ex.Message);
                }
                finally
                {
                    _buildInProgress = false;
                }
            });
        }

        /// <summary>
        /// Same approach as Spout: unsafe copy bitmap into cached pixel buffer. Returns false if no pixel data.
        /// </summary>
        private static unsafe bool CopyBitmapToRawByteArray(SKBitmap bitmap)
        {
            using var pixmap = bitmap.PeekPixels();
            if (pixmap == null)
                return false;

            SKImageInfo imageInfo = bitmap.Info;
            if (imageInfo.BytesSize <= 0 || imageInfo.Width <= 0 || imageInfo.Height <= 0)
                return false;

            imageInfo = imageInfo.WithColorType(SKColorType.Rgba8888);

            if (_pixelData == null || _pixelData.Length != imageInfo.BytesSize)
                _pixelData = new byte[imageInfo.BytesSize];

            fixed (byte* ptr = _pixelData)
            {
                pixmap.ReadPixels(imageInfo, (IntPtr)ptr, imageInfo.RowBytes, 0, 0);
            }

            _pixelWidth = imageInfo.Width;
            _pixelHeight = imageInfo.Height;
            return true;
        }

        /// <summary>
        /// Build 32-bit BMP from cached RGBA into the back buffer. Transparent pixels get the configured background color.
        /// Swap with front is done by caller so the HTTP handler never blocks on the build.
        /// </summary>
        private static unsafe void BuildBmpFromRgbaIntoBack()
        {
            if (_pixelData == null || _pixelData.Length == 0 || _pixelWidth <= 0 || _pixelHeight <= 0)
                return;

            int pixelDataSize = _pixelWidth * _pixelHeight * 4;
            int bmpSize = BmpHeaderSize + pixelDataSize;
            if (_bmpBufferBack == null || _bmpBufferBack.Length != bmpSize)
                _bmpBufferBack = new byte[bmpSize];

            WriteBmpHeader(_bmpBufferBack, _pixelWidth, _pixelHeight);

            fixed (byte* pSrc = _pixelData)
            fixed (byte* pDst = _bmpBufferBack)
            {
                byte* dstRow = pDst + BmpHeaderSize;
                int srcRowBytes = _pixelWidth * 4;
                for (int y = _pixelHeight - 1; y >= 0; y--)
                {
                    byte* src = pSrc + (y * srcRowBytes);
                    for (int x = 0; x < _pixelWidth; x++)
                    {
                        dstRow[0] = src[2];
                        dstRow[1] = src[1];
                        dstRow[2] = src[0];
                        dstRow[3] = src[3];
                        dstRow += 4;
                        src += 4;
                    }
                }
            }
        }

        private static void TryParseBackgroundColor(string? hex, out byte r, out byte g, out byte b)
        {
            r = 0;
            g = 255;
            b = 0;
            if (string.IsNullOrWhiteSpace(hex))
                return;
            string s = hex.TrimStart('#');
            if (s.Length != 6)
                return;
            if (!int.TryParse(s.AsSpan(0, 2), System.Globalization.NumberStyles.HexNumber, null, out int ri) ||
                !int.TryParse(s.AsSpan(2, 2), System.Globalization.NumberStyles.HexNumber, null, out int gi) ||
                !int.TryParse(s.AsSpan(4, 2), System.Globalization.NumberStyles.HexNumber, null, out int bi))
                return;
            r = (byte)ri;
            g = (byte)gi;
            b = (byte)bi;
        }

        private static void WriteBmpHeader(byte[] buffer, int width, int height)
        {
            int pixelDataSize = width * height * 4;
            int fileSize = BmpHeaderSize + pixelDataSize;

            buffer[0] = (byte)'B';
            buffer[1] = (byte)'M';
            WriteLe(buffer, 2, fileSize);
            WriteLe(buffer, 6, 0);
            WriteLe(buffer, 10, BmpHeaderSize);
            WriteLe(buffer, 14, 40);
            WriteLe(buffer, 18, width);
            WriteLe(buffer, 22, height);
            WriteLe(buffer, 26, (short)1);
            WriteLe(buffer, 28, (short)32);
            WriteLe(buffer, 30, 0);
            WriteLe(buffer, 34, pixelDataSize);
            WriteLe(buffer, 38, 0);
            WriteLe(buffer, 42, 0);
            WriteLe(buffer, 46, 0);
            WriteLe(buffer, 50, 0);
        }

        private static void WriteLe(byte[] buffer, int offset, int value)
        {
            buffer[offset] = (byte)value;
            buffer[offset + 1] = (byte)(value >> 8);
            buffer[offset + 2] = (byte)(value >> 16);
            buffer[offset + 3] = (byte)(value >> 24);
        }

        private static void WriteLe(byte[] buffer, int offset, short value)
        {
            buffer[offset] = (byte)value;
            buffer[offset + 1] = (byte)(value >> 8);
        }

        private static async Task ListenLoop()
        {
            if (_listener == null)
                return;

            while (_listener.IsListening)
            {
                try
                {
                    var context = await _listener.GetContextAsync();
                    _ = Task.Run(() => HandleRequest(context));
                }
                catch (HttpListenerException)
                {
                    break;
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (InvalidOperationException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Log.Debug(ex, "Web output listener: {Message}", ex.Message);
                }
            }
        }

        private static void HandleRequest(HttpListenerContext context)
        {
            try
            {
                var path = context.Request.Url?.AbsolutePath.TrimStart('/') ?? "";
                if (string.IsNullOrEmpty(path) || path == "index.html")
                {
                    ServeHtml(context.Response);
                    return;
                }
                if (path.StartsWith("frame.", StringComparison.OrdinalIgnoreCase) &&
                    (path.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) ||
                     path.EndsWith(".png", StringComparison.OrdinalIgnoreCase)))
                {
                    ServeFrameImage(context.Response);
                    return;
                }
                context.Response.StatusCode = 404;
                context.Response.Close();
                return;
            }
            catch (HttpListenerException) { }
            catch (ObjectDisposedException) { }
            catch (Exception ex)
            {
                Log.Debug(ex, "Web output request error: {Message}", ex.Message);
            }
            finally
            {
                try
                {
                    context.Response?.Close();
                }
                catch { }
            }
        }

        private static void ServeHtml(HttpListenerResponse response)
        {
            const string html = """
                <!DOCTYPE html>
                <html style="background:transparent;">
                <head><meta charset="utf-8"><title>PngifyMe</title></head>
                <body style="margin:0;background:transparent;">
                <img id="frame" src="/frame.bmp" style="display:block;width:100%;height:100%;object-fit:contain;" />
                <script>
                (function(){
                  var img = document.getElementById('frame');
                  function tick() {
                    img.src = '/frame.bmp?t=' + Date.now();
                  }
                  setInterval(tick, 33);
                })();
                </script>
                </body>
                </html>
                """;
            try
            {
                var bytes = Encoding.UTF8.GetBytes(html);
                response.ContentType = "text/html; charset=utf-8";
                response.ContentLength64 = bytes.Length;
                response.OutputStream.Write(bytes, 0, bytes.Length);
            }
            catch (HttpListenerException) { }
            catch (ObjectDisposedException) { }
            finally
            {
                try { response.Close(); } catch { }
            }
        }

        private static void ServeFrameImage(HttpListenerResponse response)
        {
            byte[]? toSend;
            lock (_bufferLock)
            {
                toSend = _bmpBufferFront;
            }
            if (toSend == null || toSend.Length == 0)
            {
                try
                {
                    response.StatusCode = 204;
                }
                catch { }
                try { response.Close(); } catch { }
                return;
            }
            try
            {
                response.ContentType = "image/bmp";
                response.ContentLength64 = toSend.Length;
                response.OutputStream.Write(toSend, 0, toSend.Length);
            }
            catch (HttpListenerException) { /* client disconnected (e.g. refresh, close tab, OBS stopped source) */ }
            catch (IOException) { /* same when I/O is aborted */ }
            catch (ObjectDisposedException) { }
            finally
            {
                try { response.Close(); } catch { }
            }
        }
    }
}
