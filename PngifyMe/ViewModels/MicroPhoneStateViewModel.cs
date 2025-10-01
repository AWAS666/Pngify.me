using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Services.CharacterSetup.Basic;
using PngifyMe.Services.Settings;
using SharpHook.Data;
using SharpHook.Native;


namespace PngifyMe.ViewModels
{
    public partial class BasicStateViewModel : ObservableObject
    {
        [ObservableProperty]
        private CharacterState state;
        private BasicSetupViewModel parent;

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

        private float entryTime;

        public float EntryTime
        {
            get => entryTime; set
            {
                SetProperty(ref entryTime, value);
                state.EntryTime = value;
            }
        }

        private float exitTime;

        public float ExitTime
        {
            get => exitTime; set
            {
                SetProperty(ref exitTime, value);
                state.ExitTime = value;
            }
        }

        private bool toggle;

        public bool ToggleAble
        {
            get => toggle; set
            {
                SetProperty(ref toggle, value);
                state.ToggleAble = value;
            }
        }

        bool hotkeyByTrigger;


        public BasicStateViewModel(CharacterState state, BasicSetupViewModel parent)
        {
            this.state = state;
            this.parent = parent;
            Name = state.Name;
            DefaultState = state.Default;

            EntryTime = state.EntryTime;
            ExitTime = state.ExitTime;

            ToggleAble = state.ToggleAble;
            SetHotkey();
        }

        public BasicStateViewModel() : this(new CharacterState(), new BasicSetupViewModel())
        {

        }

        public void OnKeyDown(KeyEventArgs e)
        {
            if (e?.Key == null)
                return;
            e.Handled = true;
            State.Trigger = new();
            State.Trigger.VirtualKeyCode = (KeyCode)Avalonia.Win32.Input.KeyInterop.VirtualKeyFromKey(e.Key);
            State.Trigger.Modifiers = (EventMask)e.KeyModifiers;
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
