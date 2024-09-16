using System.Runtime.InteropServices;

namespace HotkeySelfTest
{
    using SharpHook;
    using SharpHook.Native;
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;

    class Program
    {

        /// <summary>
        /// https://github.com/TolikPylypchuk/SharpHook
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // this sucks to debug if mouse is enabled
            var hook = new SimpleGlobalHook(GlobalHookType.Keyboard);
            hook.KeyPressed += OnKeyPressed;
            hook.KeyReleased += Hook_KeyReleased;

            hook.Run();
        }       
        private static void Hook_KeyReleased(object? sender, KeyboardHookEventArgs e)
        {
        }

        private static void OnKeyPressed(object? sender, KeyboardHookEventArgs e)
        {
            Console.WriteLine($"pressed mask: {e.RawEvent.Mask}, Keycode: {e.RawEvent.Keyboard}");

        }
    }

}
