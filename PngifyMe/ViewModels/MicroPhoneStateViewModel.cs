using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Services.Settings;
using SharpHook.Native;


namespace PngifyMe.ViewModels
{
    public partial class MicroPhoneStateViewModel : ObservableObject
    {
        [ObservableProperty]
        private MicroPhoneState state;
        private MicroPhoneSetupViewModel parent;

        private string hotkey;

        public string Hotkey
        {
            get => hotkey; set
            {
                SetProperty(ref hotkey, value);
                if (!hotkeyByTrigger)
                    State.Trigger = null;

                hotkeyByTrigger = false;
            }
        }

        private string name;

        public string Name
        {
            get => name; set
            {
                SetProperty(ref name, value);
                state.Name = value;
            }
        }

        private bool defaultState;

        public bool DefaultState
        {
            get => defaultState; set
            {
                SetProperty(ref defaultState, value);
                state.Default = value;
            }
        }

        bool hotkeyByTrigger;


        public MicroPhoneStateViewModel(MicroPhoneState state, MicroPhoneSetupViewModel parent)
        {
            this.state = state;
            this.parent = parent;
            Name = state.Name;
            DefaultState = state.Default;
            SetHotkey();
        }       

        public void OnKeyDown(KeyEventArgs e)
        {
            if (e?.Key == null)
                return;
            e.Handled = true;
            State.Trigger = new();
            State.Trigger.VirtualKeyCode = (KeyCode)Avalonia.Win32.Input.KeyInterop.VirtualKeyFromKey(e.Key);
            State.Trigger.Modifiers = (ModifierMask)e.KeyModifiers;
            SetHotkey();
        }

        public void SetHotkey()
        {
            if (State.Trigger == null)
                return;
            string text;
            if (State.Trigger.Modifiers == 0)
                text = $"{State.Trigger.VirtualKeyCode}";
            else
                text = $"{State.Trigger.Modifiers} + {State.Trigger.VirtualKeyCode}";

            hotkeyByTrigger = true;
            Hotkey = text;
        }       
    }
}
