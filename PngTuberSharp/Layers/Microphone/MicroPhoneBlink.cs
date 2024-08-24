using Avalonia.Media.Imaging;
using PngTuberSharp.Services;
using SkiaSharp;

namespace PngTuberSharp.Layers.Microphone
{
    public class MicroPhoneBlink
    {
        private SKBitmap openImage;
        private SKBitmap openBlinkImage;
        private SKBitmap closedImage;
        private SKBitmap closedBlinkImage;
        private float transTime;
        private bool blinking;

        public string Open { get; set; }
        public string? OpenBlink { get; set; } = null;
        public string Closed { get; set; }
        public string? ClosedBlink { get; set; } = null;
        public float Interval { get; set; } = 2f;
        public float Time { get; set; } = 0.25f;
        public float CurrentTime { get; private set; }

        public MicroPhoneBlink()
        {
            openImage = SKBitmap.Decode(Open);
            openBlinkImage = SKBitmap.Decode(OpenBlink ?? Open);
            closedImage = SKBitmap.Decode(Closed);
            closedBlinkImage = SKBitmap.Decode(ClosedBlink ?? Closed);

            transTime = Interval;
        }

        public void OnCalculateParameters(float dt, ref LayerValues values)
        {
            CurrentTime += dt;
            if (!blinking && CurrentTime > transTime)
            {
                transTime += Interval;
                blinking = true;
            }
            else if (blinking && CurrentTime > transTime)
            {
                transTime += Time;
                blinking = false;
            }

            if (MicrophoneService.Talking)
                values.Image = blinking ? openBlinkImage : openImage;
            else
                values.Image = blinking ? closedBlinkImage : closedImage;
        }       
    }
}
