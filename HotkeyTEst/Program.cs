using GlobalHotKeys.Native.Types;
using GlobalHotKeys;

namespace HotkeyTEst
{
    internal class Program
    {
        static void Main(string[] args)
        {
            void HotKeyPressed(HotKey hotKey) =>
                Console.WriteLine($"HotKey Pressed: Id = {hotKey.Id}, Key = {hotKey.Key}, Modifiers = {hotKey.Modifiers}");

            using var hotKeyManager = new HotKeyManager();
            using var subscription = hotKeyManager.HotKeyPressed.Subscribe(HotKeyPressed);
            using var shift1 = hotKeyManager.Register(VirtualKeyCode.VK_NUMPAD0, 0);

            Console.WriteLine("Listening for HotKeys...");
            Console.ReadLine();
        }
    }
}
