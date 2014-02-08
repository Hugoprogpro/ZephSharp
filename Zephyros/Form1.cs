using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace Zephyros
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var wins = Api.Window.GetWindows();
            foreach (var win in wins)
            {
                if (win.IsVisible())
                {
                    Rectangle r = win.GetRect();
                    r.X += 10;
                    win.Move(r);
                    Console.WriteLine("HEY = " + win.Title());
                }
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                //m.Msg
                button1_Click(null, null);
                //HotKeyEventArgs e = new HotKeyEventArgs(m.LParam);
                //HotKeyManager.OnHotKeyPressed(e);
            }

            base.WndProc(ref m);
        }

        private const int WM_HOTKEY = 0x312;

        private void Form1_Load(object sender, EventArgs e)
        {
            Api.HotKey.DoIt(this.Handle);

            clojure.lang.Var v = clojure.lang.RT.var("clojure.core", "load-file");
            v.invoke(@"C:\Users\sdegutis\Desktop\Test.clj");
        }
    }
}


namespace Api
{
    public class HotKey
    {
        public static void Greet() {
            MessageBox.Show("Hello!");
        }

        [DllImport("user32", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [Flags]
        public enum KeyModifiers
        {
            Alt = 1,
            Control = 2,
            Shift = 4,
            Windows = 8,
            NoRepeat = 0x4000
        }

        public static void DoIt(IntPtr hwnd)
        {
            RegisterHotKey(hwnd, 1, (uint)(KeyModifiers.Windows | KeyModifiers.Alt), 0x44);
        }
    }

    public class Window
    {
        private delegate bool EnumDelegate(IntPtr hWnd, int lParam);
        [DllImport("user32.dll", ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool EnumWindows(EnumDelegate lpEnumCallbackFunction, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RawRect lpRect);


        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);


        [StructLayout(LayoutKind.Sequential)]
        public struct RawRect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        
        private IntPtr ptr;

        public static List<Window> GetWindows()
        {
            var windows = new List<Window>();
            EnumDelegate filter = delegate(IntPtr hWnd, int lParam)
            {
                if (IsWindowVisible(hWnd))
                {
                    var w = new Window();
                    w.ptr = hWnd;
                    windows.Add(w);
                }
                return true;
            };

            EnumWindows(filter, IntPtr.Zero);
            return windows;
        }

        public bool IsVisible()
        {
            return IsWindowVisible(ptr) && this.Title() != "";
        }

        public string Title()
        {
            StringBuilder strbTitle = new StringBuilder(255);
            int nLength = GetWindowText(ptr, strbTitle, strbTitle.Capacity + 1);
            string strTitle = strbTitle.ToString();

            if (!string.IsNullOrEmpty(strTitle))
                return strTitle;
            else
                return "";
        }

        public Rectangle GetRect()
        {
            RawRect rct;

            if (!GetWindowRect(ptr, out rct))
                return new Rectangle();

            return new Rectangle{X = rct.Left, Y = rct.Top, Width = rct.Right - rct.Left + 1, Height = rct.Bottom - rct.Top + 1};
        }

        public bool Move(Rectangle r)
        {
            return MoveWindow(ptr, r.X, r.Y, r.Width, r.Height, true);
        }
    }
}
