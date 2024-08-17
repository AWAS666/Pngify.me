using Avalonia.Media.Imaging;
using PngTuberSharp.Services;

namespace PngTuberSharp.Layers.Microphone
{
    public class MicroPhoneBlink : PermaLayer
    {
        private Bitmap openImage;
        private Bitmap openBlinkImage;
        private Bitmap closedImage;
        private Bitmap closedBlinkImage;
        private float transTime;
        private bool blinking;

        public string Open { get; set; }
        public string? OpenBlink { get; set; } = null;
        public string Closed { get; set; }
        public string? ClosedBlink { get; set; } = null;
        public float Interval { get; set; } = 2f;
        public float Time { get; set; } = 0.25f;


        public MicroPhoneBlink()
        {
            Unique = true;
            openImage = new Bitmap(Open);
            openBlinkImage = new Bitmap(OpenBlink ?? Open);
            closedImage = new Bitmap(Closed);
            closedBlinkImage = new Bitmap(ClosedBlink ?? Closed);

            transTime = Interval;
        }

        public override void OnCalculateParameters(float dt, ref LayerValues values)
        {
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
