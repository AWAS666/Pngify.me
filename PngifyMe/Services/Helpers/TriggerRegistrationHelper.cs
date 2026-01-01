using PngifyMe.Services.Hotkey;
using PngifyMe.Services.Settings;
using PngifyMe.Services.Twitch;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PngifyMe.Services.Helpers;

public class TriggerRegistrationHelper
{
    private readonly List<Action> _hotkeyCallbacks = new();
    private readonly List<TwitchRedeem> _registeredRedeems = new();
    private readonly List<TwitchBits> _registeredBits = new();
    private readonly List<TwitchSub> _registeredSubs = new();
    private readonly List<TwitchTextCommand> _registeredTextCommands = new();
    private readonly List<TwitchRaid> _registeredRaids = new();
    private readonly List<TwitchFollow> _registeredFollows = new();

    public void RegisterTrigger(Trigger trigger, Action callback)
    {
        if (trigger == null)
            return;

        trigger.Callback = callback;

        switch (trigger)
        {
            case HotkeyTrigger hotKey:
                HotkeyManager.AddHotkey(hotKey.VirtualKeyCode, hotKey.Modifiers, callback);
                _hotkeyCallbacks.Add(callback);
                break;
            case TwitchRedeem redeem:
                TwitchEventSocket.RedeemUsed += redeem.Triggered;
                _registeredRedeems.Add(redeem);
                break;
            case TwitchBits bits:
                TwitchEventSocket.BitsUsed += bits.Triggered;
                _registeredBits.Add(bits);
                break;
            case TwitchSub subs:
                TwitchEventSocket.AnySub += subs.Triggered;
                _registeredSubs.Add(subs);
                break;
            case TwitchTextCommand command:
                TwitchEventSocket.NewChat += command.Triggered;
                _registeredTextCommands.Add(command);
                break;
            case TwitchRaid raid:
                TwitchEventSocket.Raid += raid.Triggered;
                _registeredRaids.Add(raid);
                break;
            case TwitchFollow follow:
                TwitchEventSocket.NewFollower += follow.Triggered;
                _registeredFollows.Add(follow);
                break;
            case AlwaysActive:
                break;
            default:
                break;
        }
    }

    public void RegisterTriggers<T>(IEnumerable<T> items, Func<T, Action> callbackFactory) where T : class
    {
        if (items == null)
            return;

        foreach (var item in items)
        {
            var triggerProperty = typeof(T).GetProperty("Trigger");
            if (triggerProperty == null)
                continue;

            var trigger = triggerProperty.GetValue(item) as Trigger;
            if (trigger == null)
                continue;

            var callback = callbackFactory(item);
            RegisterTrigger(trigger, callback);
        }
    }

    public void UnregisterTrigger(Trigger trigger)
    {
        if (trigger == null)
            return;

        switch (trigger)
        {
            case TwitchRedeem redeem:
                if (_registeredRedeems.Remove(redeem))
                {
                    TwitchEventSocket.RedeemUsed -= redeem.Triggered;
                }
                break;
            case TwitchBits bits:
                if (_registeredBits.Remove(bits))
                {
                    TwitchEventSocket.BitsUsed -= bits.Triggered;
                }
                break;
            case TwitchSub subs:
                if (_registeredSubs.Remove(subs))
                {
                    TwitchEventSocket.AnySub -= subs.Triggered;
                }
                break;
            case TwitchTextCommand command:
                if (_registeredTextCommands.Remove(command))
                {
                    TwitchEventSocket.NewChat -= command.Triggered;
                }
                break;
            case TwitchRaid raid:
                if (_registeredRaids.Remove(raid))
                {
                    TwitchEventSocket.Raid -= raid.Triggered;
                }
                break;
            case TwitchFollow follow:
                if (_registeredFollows.Remove(follow))
                {
                    TwitchEventSocket.NewFollower -= follow.Triggered;
                }
                break;
            default:
                break;
        }
    }

    public static void UnregisterSingleTrigger(Trigger trigger)
    {
        if (trigger == null)
            return;

        switch (trigger)
        {
            case TwitchRedeem redeem:
                TwitchEventSocket.RedeemUsed -= redeem.Triggered;
                break;
            case TwitchBits bits:
                TwitchEventSocket.BitsUsed -= bits.Triggered;
                break;
            case TwitchSub subs:
                TwitchEventSocket.AnySub -= subs.Triggered;
                break;
            case TwitchTextCommand command:
                TwitchEventSocket.NewChat -= command.Triggered;
                break;
            case TwitchRaid raid:
                TwitchEventSocket.Raid -= raid.Triggered;
                break;
            case TwitchFollow follow:
                TwitchEventSocket.NewFollower -= follow.Triggered;
                break;
            default:
                break;
        }
    }

    public void Cleanup()
    {
        HotkeyManager.RemoveCallbacks(_hotkeyCallbacks);
        _hotkeyCallbacks.Clear();

        foreach (var redeem in _registeredRedeems)
        {
            TwitchEventSocket.RedeemUsed -= redeem.Triggered;
        }
        _registeredRedeems.Clear();

        foreach (var bits in _registeredBits)
        {
            TwitchEventSocket.BitsUsed -= bits.Triggered;
        }
        _registeredBits.Clear();

        foreach (var subs in _registeredSubs)
        {
            TwitchEventSocket.AnySub -= subs.Triggered;
        }
        _registeredSubs.Clear();

        foreach (var command in _registeredTextCommands)
        {
            TwitchEventSocket.NewChat -= command.Triggered;
        }
        _registeredTextCommands.Clear();

        foreach (var raid in _registeredRaids)
        {
            TwitchEventSocket.Raid -= raid.Triggered;
        }
        _registeredRaids.Clear();

        foreach (var follow in _registeredFollows)
        {
            TwitchEventSocket.NewFollower -= follow.Triggered;
        }
        _registeredFollows.Clear();
    }
}

