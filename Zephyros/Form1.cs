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

                Console.WriteLine(m.WParam.ToString());
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
            //MessageBox.Show("Hello!");
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
            RegisterHotKey(hwnd, 72, (uint)(KeyModifiers.Windows | KeyModifiers.Alt), 0x44);
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





/* TODO: use this instead



using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace ConsoleHotKey
{
  public static class HotKeyManager
  {
    public static event EventHandler<HotKeyEventArgs> HotKeyPressed;

    public static int RegisterHotKey(Keys key, KeyModifiers modifiers)
    {
      _windowReadyEvent.WaitOne();
      int id = System.Threading.Interlocked.Increment(ref _id);
      _wnd.Invoke(new RegisterHotKeyDelegate(RegisterHotKeyInternal), _hwnd, id, (uint)modifiers, (uint)key);
      return id;
    }

    public static void UnregisterHotKey(int id)
    {
      _wnd.Invoke(new UnRegisterHotKeyDelegate(UnRegisterHotKeyInternal), _hwnd, id);
    }

    delegate void RegisterHotKeyDelegate(IntPtr hwnd, int id, uint modifiers, uint key);
    delegate void UnRegisterHotKeyDelegate(IntPtr hwnd, int id);

    private static void RegisterHotKeyInternal(IntPtr hwnd, int id, uint modifiers, uint key)
    {      
      RegisterHotKey(hwnd, id, modifiers, key);      
    }

    private static void UnRegisterHotKeyInternal(IntPtr hwnd, int id)
    {
      UnregisterHotKey(_hwnd, id);
    }    

    private static void OnHotKeyPressed(HotKeyEventArgs e)
    {
      if (HotKeyManager.HotKeyPressed != null)
      {
        HotKeyManager.HotKeyPressed(null, e);
      }
    }

    private static volatile MessageWindow _wnd;
    private static volatile IntPtr _hwnd;
    private static ManualResetEvent _windowReadyEvent = new ManualResetEvent(false);
    static HotKeyManager()
    {
      Thread messageLoop = new Thread(delegate()
        {
          Application.Run(new MessageWindow());
        });
      messageLoop.Name = "MessageLoopThread";
      messageLoop.IsBackground = true;
      messageLoop.Start();      
    }

    private class MessageWindow : Form
    {
      public MessageWindow()
      {
        _wnd = this;
        _hwnd = this.Handle;
        _windowReadyEvent.Set();
      }

      protected override void WndProc(ref Message m)
      {
        if (m.Msg == WM_HOTKEY)
        {
          HotKeyEventArgs e = new HotKeyEventArgs(m.LParam);
          HotKeyManager.OnHotKeyPressed(e);
        }

        base.WndProc(ref m);
      }

      protected override void SetVisibleCore(bool value)
      {
        // Ensure the window never becomes visible
        base.SetVisibleCore(false);
      }

      private const int WM_HOTKEY = 0x312;
    }

    [DllImport("user32", SetLastError=true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private static int _id = 0;
  }


  public class HotKeyEventArgs : EventArgs
  {
    public readonly Keys Key;
    public readonly KeyModifiers Modifiers;

    public HotKeyEventArgs(Keys key, KeyModifiers modifiers)
    {
      this.Key = key;
      this.Modifiers = modifiers;
    }

    public HotKeyEventArgs(IntPtr hotKeyParam)
    {
      uint param = (uint)hotKeyParam.ToInt64();
      Key = (Keys)((param & 0xffff0000) >> 16);
      Modifiers = (KeyModifiers)(param & 0x0000ffff);
    }
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
}


*/











/* Here's an example usage:



using System;
using System.Windows.Forms;

namespace ConsoleHotKey
{
  class Program
  {
    static void Main(string[] args)
    {
      HotKeyManager.RegisterHotKey(Keys.A, KeyModifiers.Alt);
      HotKeyManager.HotKeyPressed += new EventHandler<HotKeyEventArgs>(HotKeyManager_HotKeyPressed);
      Console.ReadLine();      
    }

    static void HotKeyManager_HotKeyPressed(object sender, HotKeyEventArgs e)
    {
      Console.WriteLine("Hit me!");
    }
  }
}

*/