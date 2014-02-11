using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Zephyros
{
    public class Window
    {
        private class WinAPI
        {
            public delegate bool EnumDelegate(IntPtr hWnd, int lParam);
            [DllImport("user32.dll", ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
            public static extern bool EnumWindows(EnumDelegate lpEnumCallbackFunction, IntPtr lParam);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool IsWindowVisible(IntPtr hWnd);

            [DllImport("user32.dll", ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
            public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetWindowRect(IntPtr hWnd, out RawRect lpRect);


            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool ShowWindow(IntPtr hwnd, int cmd);


            [StructLayout(LayoutKind.Sequential)]
            public struct RawRect
            {
                public int Left;
                public int Top;
                public int Right;
                public int Bottom;
            }

            [DllImport("user32.dll")]
            public static extern IntPtr GetForegroundWindow();
        }

        private IntPtr ptr;

        public static Window GetActiveWindow()
        {
            Window w = new Window();
            w.ptr = WinAPI.GetForegroundWindow();
            return w;
        }

        public static List<Window> GetWindows()
        {
            var windows = new List<Window>();
            WinAPI.EnumDelegate filter = delegate(IntPtr hWnd, int lParam)
            {
                if (WinAPI.IsWindowVisible(hWnd))
                {
                    var w = new Window();
                    w.ptr = hWnd;
                    windows.Add(w);
                }
                return true;
            };

            WinAPI.EnumWindows(filter, IntPtr.Zero);
            return windows;
        }

        public bool IsVisible()
        {
            return WinAPI.IsWindowVisible(ptr) && this.GetTitle() != "";
        }

        public string GetTitle()
        {
            StringBuilder strbTitle = new StringBuilder(255);
            int nLength = WinAPI.GetWindowText(ptr, strbTitle, strbTitle.Capacity + 1);
            string strTitle = strbTitle.ToString();

            if (!string.IsNullOrEmpty(strTitle))
                return strTitle;
            else
                return "";
        }

        public Rectangle GetRect()
        {
            WinAPI.RawRect rct;

            if (!WinAPI.GetWindowRect(ptr, out rct))
                return new Rectangle();

            return new Rectangle { X = rct.Left, Y = rct.Top, Width = rct.Right - rct.Left + 1, Height = rct.Bottom - rct.Top + 1 };
        }

        public bool Move(int x, int y, int width, int height)
        {
            WinAPI.ShowWindow(ptr, SW_RESTORE);
            return WinAPI.MoveWindow(ptr, x, y, width, height, true);
        }

        private const int SW_RESTORE = 9;
    }
}
