using PngifyMe.Layers;
using PngifyMe.Services.CharacterSetup.Basic;
using PngifyMe.Services.CharacterSetup.Images;
using SkiaSharp;

namespace PngifyMe.Services.CharacterSetup
{
    public interface ICharacterSetup
    {
        BaseImage CurrentImage { get; }
        IAvatarSettings Settings { get; set; }

        void DrawTransition(SKBitmap baseImg, int width, int height, SKCanvas canvas, float opacity);
        void RefreshCharacterSettings();
        void SetupHotKeys();
        void ToggleState(string state);
        void Update(float dt, ref LayerValues values);

        bool RenderPosition { get; }
        bool RefreshCollisionOnChange { get; }
    }
}