using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Linq;

namespace Zephyros
{
    public class HotKey
    {
        [DllImport("user32", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private KeyModifiers translateMods(IEnumerable mods)
        {
            KeyModifiers keyMods = 0;
            foreach (clojure.lang.Keyword mod in mods)
            {
                var str = mod.Name;
                Console.WriteLine(mod.ToString());
                if (str.ToUpper() == "ALT") keyMods |= KeyModifiers.Alt;
                if (str.ToUpper() == "CTRL") keyMods |= KeyModifiers.Control;
                if (str.ToUpper() == "SHIFT") keyMods |= KeyModifiers.Shift;
                if (str.ToUpper() == "WIN") keyMods |= KeyModifiers.Windows;
                if (str.ToUpper() == "NOREPEAT") keyMods |= KeyModifiers.NoRepeat;
            }
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

        public HotKey(string key, IEnumerable mods)
        {
            uint rawMods = (uint)translateMods(mods);
            uint rawKey = key.ToUpper().ToCharArray()[0];
            IntPtr hwnd = CallbackWindow.SharedCallbackWindow.Handle;

            Console.WriteLine("[{0}, {1}]", rawMods.ToString(), rawKey.ToString());

            //rawMods = (uint)(KeyModifiers.Alt | KeyModifiers.Control);
            //rawKey = 0x44;

            hotkeyID = ++nextHotkeyID;
            RegisterHotKey(hwnd, hotkeyID, rawMods, rawKey);
            Console.WriteLine("here rgh now");
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
                    Console.WriteLine(hotkey.ToString());
                    bool handled = hotkey.OnHotKeyPressed();
                }

                base.WndProc(ref m);
            }

            private const int WM_HOTKEY = 0x312;
        }
    }
}
