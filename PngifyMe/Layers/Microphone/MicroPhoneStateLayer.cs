using PngifyMe.Services;
using PngifyMe.Services.Hotkey;
using PngifyMe.Services.Settings;
using PngifyMe.Services.Settings.Images;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PngifyMe.Layers.Microphone;

public class MicroPhoneStateLayer
{
    private BaseImage openImage;
    private BaseImage openBlinkImage;
    private BaseImage closedImage;
    private BaseImage closedBlinkImage;

    private BaseImage enterImage;
    private BaseImage exitImage;

    public MicroPhoneState CurrentState { get; private set; }
    private MicroPhoneState lastState;
    private float transTime;
    private MicroPhoneSettings micSettings;
    private bool blinking;
    private List<Action> callbacks = new();
    public BaseImage CurrentImage { get; private set; }

    public double? BlendTime { get; private set; }

    public float CurrentTime { get; private set; }
    public float EntryTime { get; private set; }
    public float ExitTime { get; private set; }
    public BaseImage LastImage { get; private set; }

    public MicroPhoneStateLayer()
    {
        RefreshMicSettings();
        SettingsManager.Current.Profile.PropertyChanged += Profile_PropertyChanged;
    }

    private void Profile_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        RefreshMicSettings();
    }

    private void RefreshMicSettings()
    {
        micSettings = SettingsManager.Current.Profile.Active.MicroPhone;
        var def = micSettings.States.FirstOrDefault(x => x.Default);
        if (def == null)
        {
            def = micSettings.States.First();
            def.Default = true;
        }
        SwitchState(def);
        SetupHotKeys();
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
            BlendTime = CurrentTime + micSettings.TransitionTime;
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
            transTime += (float)micSettings.BlinkTime;
            blinking = true;
        }
        else if (blinking && CurrentTime > transTime)
        {
            transTime += (float)micSettings.BlinkInterval;
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

    public void SwitchState(MicroPhoneState state, bool reload = false)
    {
        if (CurrentState == state && !reload) return;

        openImage = state.Open.Bitmap;
        openBlinkImage = !string.IsNullOrEmpty(state.OpenBlink.FilePath) ? state.OpenBlink.Bitmap : state.Open.Bitmap;
        closedImage = state.Closed.Bitmap;
        closedBlinkImage = !string.IsNullOrEmpty(state.ClosedBlink.FilePath) ? state.ClosedBlink.Bitmap : state.Closed.Bitmap;
        lastState = CurrentState;
        CurrentState = state;

        enterImage = state.EntryImage.Bitmap;
        exitImage = state.ExitImage.Bitmap;

        if (lastState != null)
            ExitTime = CurrentTime + lastState.ExitTime;
        EntryTime = ExitTime + CurrentState.EntryTime;
    }

    public void ToggleState(MicroPhoneState state)
    {
        if (CurrentState == state && CurrentState.ToggleAble)
        {
            SwitchState(micSettings.States.First(x => x.Default));
        }
        else if (CurrentState != state)
        {
            SwitchState(state);
        }
    }

    public void SetupHotKeys()
    {
        HotkeyManager.RemoveCallbacks(callbacks);
        callbacks.Clear();
        foreach (var state in micSettings.States)
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
}
