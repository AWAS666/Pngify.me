using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using GlobalHotKeys.Native.Types;
using PngTuberSharp.Services.Settings;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace PngTuberSharp.ViewModels.Helper
{
    public partial class TriggerViewModel : ObservableObject
    {
        [ObservableProperty]
        private Trigger trigger;

        [ObservableProperty]
        private ObservableCollection<VirtualKeyCode> allKeys;

        [ObservableProperty]
        private ObservableCollection<Modifiers> allModifiers;

        private string redeem;

        public string Redeem
        {
            get => redeem; set
            {
                ((TwitchTrigger)trigger).Redeem = value;
                SetProperty(ref redeem, value);
            }
        }

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
                case TwitchTrigger twitch:
                    Redeem = twitch.Redeem;
                    break;
                case HotkeyTrigger hotkey:
                    AllKeys = new ObservableCollection<VirtualKeyCode>(Enum.GetValues(typeof(VirtualKeyCode)).Cast<VirtualKeyCode>());
                    AllModifiers = new ObservableCollection<Modifiers>(Enum.GetValues(typeof(Modifiers)).Cast<Modifiers>());
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
            hot.VirtualKeyCode = (VirtualKeyCode)Avalonia.Win32.Input.KeyInterop.VirtualKeyFromKey(e.Key);
            hot.Modifiers = (Modifiers)e.KeyModifiers;
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
