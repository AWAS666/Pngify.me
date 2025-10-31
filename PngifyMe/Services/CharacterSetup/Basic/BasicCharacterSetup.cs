using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Layers;
using PngifyMe.Services.CharacterSetup.Images;
using PngifyMe.Services.Hotkey;
using SkiaSharp;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;

namespace PngifyMe.Services.CharacterSetup.Basic;

public class BasicCharacterSetup : ICharacterSetup
{

    private BaseImage openImage;
    private BaseImage openBlinkImage;
    private BaseImage closedImage;
    private BaseImage closedBlinkImage;

    private BaseImage enterImage;
    private BaseImage exitImage;

    public CharacterState CurrentState { get; private set; }
    private CharacterState lastState;

    private double transTime;
    private bool blinking;
    private List<Action> callbacks = new();
    public BaseImage CurrentImage { get; private set; }

    public double? BlendTime { get; private set; }

    public float CurrentTime { get; private set; }
    public float EntryTime { get; private set; }
    public float ExitTime { get; private set; }
    public BaseImage LastImage { get; private set; }
    public IAvatarSettings Settings { get; set; }
    private BasicCharSettings settings => (BasicCharSettings)Settings;
    public bool RenderPosition => true;
    public bool RefreshCollisionOnChange => true;

    public EventHandler<CharacterState> StateChanged;

    public void RefreshCharacterSettings()
    {
        var def = settings.States.FirstOrDefault(x => x.Default);
        if (def == null)
        {
            def = settings.States.First();
            def.Default = true;
        }
        SwitchState(def);
        SetupHotKeys();
    }

    public void SwitchState(CharacterState state, bool reload = false)
    {
        if (CurrentState == state && !reload) return;

        openImage = state.Open.Bitmap;
        openBlinkImage = state.OpenBlink.Base64.Count > 0 ? state.OpenBlink.Bitmap : state.Open.Bitmap;
        closedImage = state.Closed.Bitmap;
        closedBlinkImage = state.ClosedBlink.Base64.Count > 0 ? state.ClosedBlink.Bitmap : state.Closed.Bitmap;
        lastState = CurrentState;
        CurrentState = state;

        enterImage = state.EntryImage.Bitmap;
        exitImage = state.ExitImage.Bitmap;

        if (lastState != null)
            ExitTime = CurrentTime + lastState.ExitTime;
        EntryTime = ExitTime + CurrentState.EntryTime;

        StateChanged?.Invoke(this, state);
    }

    public void SetupHotKeys()
    {
        HotkeyManager.RemoveCallbacks(callbacks);
        callbacks.Clear();
        foreach (var state in settings.States)
        {
            if (state.Trigger == null)
                continue;
            var callback = () => ToggleState(state);
            HotkeyManager.AddHotkey(state.Trigger.VirtualKeyCode, state.Trigger.Modifiers, callback);
            callbacks.Add(callback);
        }
        // also reloads current just in case
        SwitchState(CurrentState, reload: true);
    }

    public void ToggleState(string name)
    {
        var state = settings.States.First(x => x.Name == name);
        ToggleState(state);
    }

    public void ToggleState(CharacterState state)
    {
        if (CurrentState == state && CurrentState.ToggleAble)
        {
            SwitchState(settings.States.First(x => x.Default));
        }
        else if (CurrentState != state)
        {
            SwitchState(state);
        }
    }

    public void Update(float dt, ref LayerValues values)
    {
        CurrentTime += dt;

        if (CurrentTime < ExitTime)
        {
            BaseImage baseImage = lastState.ExitImage.Bitmap;
            values.Image = baseImage.GetImage(TimeSpan.FromSeconds(CurrentTime));
        }
        else if (CurrentTime < EntryTime)
        {
            BaseImage baseImage = enterImage;
            values.Image = baseImage.GetImage(TimeSpan.FromSeconds(CurrentTime));
        }
        else
        {
            CurrentImage = Baseupdate(ref values);
        }


        if (LastImage != CurrentImage && BlendTime == null)
        {
            BlendTime = CurrentTime + settings.TransitionTime;
        }
        else if (BlendTime < CurrentTime)
        {
            LastImage = CurrentImage;
            BlendTime = null;
        }
    }

    private BaseImage Baseupdate(ref LayerValues values)
    {
        if (!blinking && CurrentTime > transTime)
        {
            transTime += settings.BlinkTime;
            blinking = true;
        }
        else if (blinking && CurrentTime > transTime)
        {
            var variance = Random.Shared.NextDouble() * settings.BlinkIntervalVariance;
            transTime += settings.BlinkInterval + variance;
            blinking = false;
        }

        BaseImage baseImage;
        if (AudioService.Talking)
            baseImage = blinking ? openBlinkImage : openImage;
        else
            baseImage = blinking ? closedBlinkImage : closedImage;
        values.Image = baseImage.GetImage(TimeSpan.FromSeconds(CurrentTime));
        return baseImage;
    }

    public void DrawTransition(SKBitmap baseImg, int width, int height, SKCanvas canvas, float opacity)
    {
        if (settings.TransitionTime == 0f) return;
        if (BlendTime == null || LastImage == null) return;

        var blend = (BlendTime - CurrentTime) / settings.TransitionTime;
        // factor in blending of main layer
        blend *= opacity;
        using (SKPaint paint = new SKPaint { Color = SKColors.White.WithAlpha((byte)(blend * 255)) })
        {
            canvas.DrawBitmap(LastImage.GetImage(TimeSpan.FromSeconds(CurrentTime)),
                width / 2 - baseImg.Width / 2,
                height / 2 - baseImg.Height / 2,
                paint);
        }
    }
}
