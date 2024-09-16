using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Services.Settings;
using SharpHook.Native;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace PngifyMe.ViewModels.Helper
{
    public partial class TriggerViewModel : ObservableObject
    {
        [ObservableProperty]
        private Trigger trigger;

        [ObservableProperty]
        private ObservableCollection<KeyCode> allKeys;

        [ObservableProperty]
        private ObservableCollection<ModifierMask> allModifiers;

        private string hotkey;

        public string Hotkey
        {
            get => hotkey; set
            {
                SetProperty(ref hotkey, value);
                if (!hotkeyByTrigger)
                {
                    var hot = (HotkeyTrigger)Trigger;
                    hot.VirtualKeyCode = 0;
                    hot.Modifiers = 0;
                }

                hotkeyByTrigger = false;
            }
        }

        bool hotkeyByTrigger;


        public TriggerViewModel(Trigger trigger)
        {
            Trigger = trigger;
            switch (trigger)
            {
                case HotkeyTrigger hotkey:
                    AllKeys = new ObservableCollection<KeyCode>(Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>());
                    AllModifiers = new ObservableCollection<ModifierMask>(Enum.GetValues(typeof(ModifierMask)).Cast<ModifierMask>());
                    AllModifiers.Insert(0, 0);
                    SetHotkey();
                    break;
                default:
                    break;
            }
        }


        public void OnKeyDown(KeyEventArgs e)
        {
            if (e?.Key == null)
                return;
            e.Handled = true;
            var hot = (HotkeyTrigger)Trigger;
            hot.VirtualKeyCode = (KeyCode)Avalonia.Win32.Input.KeyInterop.VirtualKeyFromKey(e.Key);
            hot.Modifiers = (ModifierMask)e.KeyModifiers;
            SetHotkey();
        }

        public void SetHotkey()
        {
            string text;
            var hot = (HotkeyTrigger)Trigger;
            if (hot.Modifiers == 0)
                text = $"{hot.VirtualKeyCode}";
            else
                text = $"{hot.Modifiers} + {hot.VirtualKeyCode}";

            hotkeyByTrigger = true;
            Hotkey = text;
        }
    }
}
