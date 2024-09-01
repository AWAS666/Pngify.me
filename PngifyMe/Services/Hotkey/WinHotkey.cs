using GlobalHotKeys;
using GlobalHotKeys.Native.Types;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;

namespace PngifyMe.Services.Hotkey
{
    public static class WinHotkey
    {
        private static HotKeyManager hotKeyManager;
        private static IDisposable subscription;

        //private static List<IRegistration> subscriptions = new List<IRegistration>();
        private static new Dictionary<(VirtualKeyCode, Modifiers), List<Action>> callBacks = new();
        private static new Dictionary<(VirtualKeyCode, Modifiers), IRegistration> hotkeys = new();

        public static void Start(Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            hotKeyManager = new HotKeyManager();
            subscription = hotKeyManager.HotKeyPressed
                .Subscribe(HotkeyTriggered);
            desktop.Exit += Desktop_Exit;
        }

        private static void HotkeyTriggered(HotKey hotKey)
        {
            Debug.WriteLine($"HotKey Pressed: Id = {hotKey.Id}, Key = {hotKey.Key}, Modifiers = {hotKey.Modifiers}");
            Log.Debug($"HotKey Pressed: Id = {hotKey.Id}, Key = {hotKey.Key}, Modifiers = {hotKey.Modifiers}");
            if (callBacks.TryGetValue((hotKey.Key, hotKey.Modifiers), out var actions))
            {
                foreach (var action in actions)
                {
                    action();
                }
            }
        }

        private static void Desktop_Exit(object? sender, Avalonia.Controls.ApplicationLifetimes.ControlledApplicationLifetimeExitEventArgs e)
        {
            hotKeyManager.Dispose();
            subscription.Dispose();
            hotkeys.Values.ToList().ForEach(s => s.Dispose());
        }

        public static void AddHotkey(VirtualKeyCode virtualKeyCode, Modifiers modifier, Action callback)
        {
            // just add to existing callbacks
            if (callBacks.TryGetValue((virtualKeyCode, modifier), out var actions))
            {
                actions.Add(callback);
                return;
            }
            var hotKeySubscription = hotKeyManager.Register(virtualKeyCode, modifier);
            hotkeys.Add((virtualKeyCode, modifier), hotKeySubscription);
            callBacks.Add((virtualKeyCode, modifier), [callback]);
        }

        public static void RemoveCallbacks(List<Action> actions)
        {
            foreach (var item in callBacks.ToList())
            {
                foreach (var action in actions)
                {
                    item.Value.Remove(action);
                }
                if (item.Value.Count == 0)
                {
                    callBacks.Remove(item.Key);

                    // dispose subscribtion objects
                    hotkeys[item.Key].Dispose();
                    hotkeys.Remove(item.Key);
                }
            }
        }
    }
}
