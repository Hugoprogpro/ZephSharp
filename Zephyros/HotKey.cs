using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Zephyros
{
    public class HotKey
    {
        static private uint translateMods(IEnumerable mods)
        {
            Modifiers keyMods = 0;
            foreach (clojure.lang.Keyword mod in mods)
            {
                var str = mod.Name;
                if (str.ToUpper() == "ALT") keyMods |= Modifiers.Alt;
                if (str.ToUpper() == "CTRL") keyMods |= Modifiers.Control;
                if (str.ToUpper() == "SHIFT") keyMods |= Modifiers.Shift;
                if (str.ToUpper() == "WIN") keyMods |= Modifiers.Windows;
                if (str.ToUpper() == "NOREPEAT") keyMods |= Modifiers.NoRepeat;
            }
            return (uint)keyMods;
        }

        [Flags]
        public enum Modifiers
        {
            Alt = 1,
            Control = 2,
            Shift = 4,
            Windows = 8,
            NoRepeat = 0x4000
        }

        private int _id;
        private uint _mods;
        private uint _key;
        private clojure.lang.IFn _callback;

        public bool InvokeCallback()
        {
            _callback.invoke();
            return false;
        }

        public HotKey(string key, IEnumerable mods, clojure.lang.IFn callback)
        {
            _id = ++ CallbackWindow.Instance.nextHotkeyID;
            _callback = callback;
            _mods = translateMods(mods);
            _key = key.ToUpper().ToCharArray()[0];
        }

        [DllImport("user32", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        public void Enable()
        {
            RegisterHotKey(CallbackWindow.Instance.Handle, _id, _mods, _key);
            CallbackWindow.Instance.HotKeyTable.Add(_id, this);
        }

        [DllImport("user32", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public void Disable()
        {
            UnregisterHotKey(CallbackWindow.Instance.Handle, _id);
            CallbackWindow.Instance.HotKeyTable.Remove(_id);
        }

        private class CallbackWindow : Form
        {
            static public readonly CallbackWindow Instance = new CallbackWindow();
            public int nextHotkeyID = 0;
            public readonly Dictionary<int, HotKey> HotKeyTable = new Dictionary<int, HotKey>();

            //protected override void SetVisibleCore(bool value)
            //{
            //    base.SetVisibleCore(false);
            //}

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_HOTKEY)
                {
                    HotKey hotkey = HotKeyTable[m.WParam.ToInt32()];
                    bool handled = hotkey.InvokeCallback();
                }

                base.WndProc(ref m);
            }

            private const int WM_HOTKEY = 0x312;
        }
    }
}
