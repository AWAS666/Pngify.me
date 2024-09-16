using Serilog;
using SharpHook;
using SharpHook.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;

namespace PngifyMe.Services.Hotkey
{

    public static class HotkeyManager
    {
        private static SimpleGlobalHook hook;

        //private static List<IRegistration> subscriptions = new List<IRegistration>();
        private static new Dictionary<(KeyCode, ModifierMask), List<Action>> callBacks = new();

        public static bool Started { get; private set; }

        /// <summary>
        /// https://sharphook.tolik.io/v5.3.7/articles/keycodes.html these need to match
        /// </summary>
        /// <param name="desktop"></param>
        public static void Start(Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            hook = new SimpleGlobalHook(GlobalHookType.Keyboard);
            hook.KeyPressed += OnKeyPressed;
            desktop.Exit += Desktop_Exit;
            Started = true;
            hook.RunAsync();
        }

        private static void OnKeyPressed(object? sender, KeyboardHookEventArgs e)
        {
            Debug.WriteLine($"HotKey Pressed: Time = {e.RawEvent.Time}, Key = {e.RawEvent.Keyboard}, Modifiers = {e.RawEvent.Mask}");
            if (callBacks.TryGetValue((e.RawEvent.Keyboard.KeyCode, e.RawEvent.Mask), out var actions))
            {
                foreach (var action in actions)
                {
                    action();
                }
            }
        }       

        private static void Desktop_Exit(object? sender, Avalonia.Controls.ApplicationLifetimes.ControlledApplicationLifetimeExitEventArgs e)
        {
            hook?.Dispose();
        }

        public static void AddHotkey(KeyCode virtualKeyCode, ModifierMask modifier, Action callback)
        {
            if (!Started) return;
            // just add to existing callbacks
            if (callBacks.TryGetValue((virtualKeyCode, modifier), out var actions))
            {
                actions.Add(callback);
                return;
            }
            callBacks.Add((virtualKeyCode, modifier), [callback]);
        }

        public static void RemoveCallbacks(List<Action> actions)
        {
            if (!Started) return;
            foreach (var item in callBacks.ToList())
            {
                foreach (var action in actions)
                {
                    item.Value.Remove(action);
                }               
            }
        }
    }
}
