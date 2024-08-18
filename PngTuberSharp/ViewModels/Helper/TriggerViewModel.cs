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

        public TriggerViewModel(Trigger trigger)
        {
            Trigger = trigger;

            AllKeys = new ObservableCollection<VirtualKeyCode>(Enum.GetValues(typeof(VirtualKeyCode)).Cast<VirtualKeyCode>());
            AllModifiers = new ObservableCollection<Modifiers>(Enum.GetValues(typeof(Modifiers)).Cast<Modifiers>());
            AllModifiers.Insert(0, 0);
        }

        [ObservableProperty]
        private ObservableCollection<VirtualKeyCode> allKeys;

        [ObservableProperty]
        private ObservableCollection<Modifiers> allModifiers;
    }
}
