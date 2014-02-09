using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Zephyros
{
    public class HotKey
    {
        [DllImport("user32", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private KeyModifiers translateMods(IEnumerable<string> mods)
        {
            KeyModifiers keyMods = 0;
            mods = mods.Select(mod => mod.ToUpper()).ToList();
            if (mods.Contains("ALT")) keyMods |= KeyModifiers.Alt;
            if (mods.Contains("CTRL")) keyMods |= KeyModifiers.Control;
            if (mods.Contains("SHIFT")) keyMods |= KeyModifiers.Shift;
            if (mods.Contains("WINDOWS")) keyMods |= KeyModifiers.Windows;
            if (mods.Contains("NOREPEAT")) keyMods |= KeyModifiers.NoRepeat;
            return keyMods;
        }

        [Flags]
        public enum KeyModifiers
        {
            Alt = 1,
            Control = 2,
            Shift = 4,
            Windows = 8,
            NoRepeat = 0x4000
        }

        private static int nextHotkeyID = 0;
        private int hotkeyID;

        public bool OnHotKeyPressed()
        {
            Console.WriteLine("called!!");
            return false;
        }

        public HotKey(string key, IEnumerable<string> mods)
        {
            uint rawMods = (uint)translateMods(mods);
            uint rawKey = key.ToUpper().First();
            IntPtr hwnd = CallbackWindow.SharedCallbackWindow.Handle;

            hotkeyID = ++nextHotkeyID;
            RegisterHotKey(hwnd, hotkeyID, rawMods, rawKey);
            CallbackWindow.SharedCallbackWindow.HotKeyTable.Add(hotkeyID, this);
        }

        private class CallbackWindow : Form
        {
            static public readonly CallbackWindow SharedCallbackWindow = new CallbackWindow();

            public readonly Dictionary<int, HotKey> HotKeyTable = new Dictionary<int, HotKey>();

            protected override void SetVisibleCore(bool value)
            {
                base.SetVisibleCore(false);
            }

            protected override void WndProc(ref Message m)
            {
                Console.WriteLine("got here");
                if (m.Msg == WM_HOTKEY)
                {
                    HotKey hotkey = HotKeyTable[m.WParam.ToInt32()];
                    bool handled = hotkey.OnHotKeyPressed();
                }

                base.WndProc(ref m);
            }

            private const int WM_HOTKEY = 0x312;
        }
    }
}
