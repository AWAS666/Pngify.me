using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Services.CharacterSetup.Basic;
using System;
using System.Linq;

namespace PngifyMe.ViewModels
{
    public partial class BasicStateViewModel : ObservableObject
    {
        [ObservableProperty]
        private CharacterState state;
        private BasicSetupViewModel parent;

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

        public BasicStateViewModel(CharacterState state, BasicSetupViewModel parent)
        {
            this.state = state;
            this.parent = parent;
            Name = state.Name;
            DefaultState = state.Default;

            EntryTime = state.EntryTime;
            ExitTime = state.ExitTime;

            ToggleAble = state.ToggleAble;
        }

        public BasicStateViewModel() : this(new CharacterState(), new BasicSetupViewModel())
        {

        }
    }
}
