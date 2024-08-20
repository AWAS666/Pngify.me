using CommunityToolkit.Mvvm.ComponentModel;
using PngTuberSharp.Layers;
using PngTuberSharp.Services;
using PngTuberSharp.Services.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Avalonia.Input;
using System.Net;


namespace PngTuberSharp.ViewModels
{
    public partial class MicroPhoneSetupViewModel : ObservableObject
    {
        public Func<IStorageProvider> GetStorageProvider { get; }

        private List<MicroPhoneState> baseStates;

        public MicroPhoneSettings Settings { get; }

        [ObservableProperty]
        private ObservableCollection<MicroPhoneStateViewModel> states;
        private Func<IStorageProvider> getStorage;

        public MicroPhoneSetupViewModel() : this(null)
        {

        }

        public MicroPhoneSetupViewModel(Func<IStorageProvider> getStorage)
        {
            GetStorageProvider = getStorage;
            baseStates = SettingsManager.Current.Microphone.States;
            Settings = SettingsManager.Current.Microphone;
            states = new ObservableCollection<MicroPhoneStateViewModel>(baseStates.Select(x => new MicroPhoneStateViewModel(x, this)));
        }

        public void Add()
        {
            var set = new MicroPhoneState();
            States.Add(new MicroPhoneStateViewModel(set, this));
            baseStates.Add(set);
        }

        public void Remove(MicroPhoneStateViewModel vm)
        {
            // dont allow removal of final state
            if (States.Count <= 1)
                return;
            States.Remove(vm);
            baseStates.Remove(vm.State);
        }

        public void SwitchState(MicroPhoneStateViewModel vm)
        {
            LayerManager.MicroPhoneStateLayer.SwitchState(vm.State);
        }

        public void Apply()
        {
            SettingsManager.Save();
            LayerManager.MicroPhoneStateLayer.SetupHotKeys();
        }

    }

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

        bool hotkeyByTrigger;


        public MicroPhoneStateViewModel(MicroPhoneState state, MicroPhoneSetupViewModel parent)
        {
            this.state = state;
            this.parent = parent;

        }

        public async Task LoadFile(ImageSetting set)
        {
            var path = await parent.GetStorageProvider().OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                Title = "Select an Image",
                FileTypeFilter = new[] { ImageAll },
                AllowMultiple = false
            });
            set.FilePath = WebUtility.UrlDecode(path.FirstOrDefault()?.Path?.AbsolutePath);
        }

        public void Delete(ImageSetting set)
        {
            set.FilePath = string.Empty;
        }

        public void OnKeyDown(KeyEventArgs e)
        {
            if (e?.Key == null)
                return;
            e.Handled = true;
            State.Trigger = new();
            State.Trigger.VirtualKeyCode = (GlobalHotKeys.Native.Types.VirtualKeyCode)Avalonia.Win32.Input.KeyInterop.VirtualKeyFromKey(e.Key);
            State.Trigger.Modifiers = (GlobalHotKeys.Native.Types.Modifiers)e.KeyModifiers;
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

        public static FilePickerFileType ImageAll { get; } = new("All Images")
        {
            Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.gif", "*.bmp", "*.webp" },
            AppleUniformTypeIdentifiers = new[] { "public.image" },
            MimeTypes = new[] { "image/*" }
        };
    }
}
