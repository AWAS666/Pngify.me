using Avalonia.Controls;
using PngifyMe.Layers;
using PngifyMe.Services.CharacterSetup.Basic;
using PngifyMe.Services.CharacterSetup.Images;
using SkiaSharp;

namespace PngifyMe.Services.CharacterSetup.Advanced;

public class SpriteCharacterSetup : ICharacterSetup
{
    public BaseImage CurrentImage { get; set; } = new StaticImage(ImageSetting.PlaceHolder);

    public IAvatarSettings Settings { get; set; }
    private SpriteCharacterSettings settings => (SpriteCharacterSettings)Settings;

    public void DrawTransition(SKBitmap baseImg, int width, int height, SKCanvas canvas, float opacity)
    {
    }

    public void RefreshCharacterSettings()
    {
    }

    public void SetupHotKeys()
    {
    }

    public void ToggleState(CharacterState state)
    {
    }

    public void Update(float dt, ref LayerValues values)
    {
        values.Image = ImageSetting.PlaceHolder;
    }
}
