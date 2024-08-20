﻿using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using GlobalHotKeys.Native.Types;
using PngTuberSharp.Layers;
using PngTuberSharp.Services.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

            AllKeys = new ObservableCollection<VirtualKeyCode>(Enum.GetValues(typeof(VirtualKeyCode)).Cast<VirtualKeyCode>());
            AllModifiers = new ObservableCollection<Modifiers>(Enum.GetValues(typeof(Modifiers)).Cast<Modifiers>());
            AllModifiers.Insert(0, 0);
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
