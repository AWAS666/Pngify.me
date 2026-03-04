using PngifyMe.Services;
using PngifyMe.Services.CharacterSetup.Advanced;
using PngifyMe.Services.Settings;
using System;

namespace PngifyMe.ViewModels.Helper;

/// <summary>
/// Canvas overlay for editing a sprite's Position (X,Y) or Anchor (AnchorX, AnchorY). No reflection; works directly with <see cref="SpriteImage"/>.
/// Position uses center of the layer; anchor uses the point as-is. Transform (translate + global zoom) is applied here so the handle aligns with the renderer.
/// </summary>
public sealed partial class SpritePointOverlayViewModel : CanvasPointOverlayViewModelBase
{
    private readonly SpriteImage _sprite;
    private readonly bool _isAnchor;
    private readonly int _positionCenterOffsetX;
    private readonly int _positionCenterOffsetY;

    public SpriteImage Sprite => _sprite;
    public bool IsAnchorMode => _isAnchor;

    public SpritePointOverlayViewModel(SpriteImage sprite, bool isAnchor)
    {
        _sprite = sprite ?? throw new ArgumentNullException(nameof(sprite));
        _isAnchor = isAnchor;

        if (!_isAnchor && _sprite.Bitmap != null)
        {
            _positionCenterOffsetX = _sprite.Bitmap.Width / 2;
            _positionCenterOffsetY = _sprite.Bitmap.Height / 2;
        }

        BeginPositionRefresh();
        try
        {
            if (_isAnchor)
            {
                X = _sprite.CurrentAnchor.X;
                Y = _sprite.CurrentAnchor.Y;
            }
            else
            {
                X = _sprite.X + _positionCenterOffsetX;
                Y = _sprite.Y + _positionCenterOffsetY;
            }
        }
        finally
        {
            EndPositionRefresh();
        }
    }

    /// <summary>
    /// Matches SpriteCharacterSetup: Translate(tx,ty) then Scale(zoom, 960, 540). So bitmap = (tx + 960 + (logical - 960)*zoom, ty + 540 + (logical - 540)*zoom).
    /// </summary>
    public override (float bitmapX, float bitmapY) ToBitmapPosition(float logicalX, float logicalY)
    {
        if (SettingsManager.Current?.Profile?.Active?.AvatarSettings is not SpriteCharacterSettings settings)
            return (logicalX, logicalY);
        float tx = (Specsmanager.Width - Specsmanager.Height) / 2f + settings.OffsetX;
        float ty = settings.OffsetY;
        float zoom = settings.Zoom;
        float px = Specsmanager.Width / 2f;
        float py = Specsmanager.Height / 2f;
        float bx = tx + px + (logicalX - px) * zoom;
        float by = ty + py + (logicalY - py) * zoom;
        return (bx, by);
    }

    public override (float logicalX, float logicalY) FromBitmapPosition(float bitmapX, float bitmapY)
    {
        if (SettingsManager.Current?.Profile?.Active?.AvatarSettings is not SpriteCharacterSettings settings)
            return (bitmapX, bitmapY);
        float tx = (Specsmanager.Width - Specsmanager.Height) / 2f + settings.OffsetX;
        float ty = settings.OffsetY;
        float zoom = settings.Zoom;
        float px = Specsmanager.Width / 2f;
        float py = Specsmanager.Height / 2f;
        float logicalX = (bitmapX - tx - px) / zoom + px;
        float logicalY = (bitmapY - ty - py) / zoom + py;
        return (logicalX, logicalY);
    }

    protected override void PushToModel()
    {
        if (_isAnchor)
        {
            _sprite.AnchorX = X - _sprite.RotOffset.X;
            _sprite.AnchorY = Y - _sprite.RotOffset.Y;
        }
        else
        {
            _sprite.X = X - _positionCenterOffsetX;
            _sprite.Y = Y - _positionCenterOffsetY;
        }
    }
}
