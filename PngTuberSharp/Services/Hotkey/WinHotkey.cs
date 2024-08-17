using GlobalHotKeys;
using GlobalHotKeys.Native.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PngTuberSharp.Services.Hotkey
{
    public static class WinHotkey
    {
        private static HotKeyManager hotKeyManager;
        private static List<IRegistration> subscriptions = new List<IRegistration>();
        private static new Dictionary<(VirtualKeyCode, Modifiers), List<Action>> callBacks = new();

        public static void Start(Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            hotKeyManager = new HotKeyManager();
            hotKeyManager.HotKeyPressed.Subscribe(HotkeyTriggered);
            desktop.Exit += Desktop_Exit;
        }

        private static void HotkeyTriggered(HotKey hotKey)
        {
            Debug.WriteLine($"HotKey Pressed: Id = {hotKey.Id}, Key = {hotKey.Key}, Modifiers = {hotKey.Modifiers}");
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
            subscriptions.ForEach(s => s.Dispose());
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
            subscriptions.Add(hotKeySubscription);
            callBacks.Add((virtualKeyCode, modifier), [callback]);
        }
    }
}
