/*
 * WinDrag, Linux Desktop Environments-Like Alt+Drag Window Mover
 * WinDrag allows you moving windows for dragging from anywhere in the window with Alt key
 *
 * http://oguzhaneroglu.com/projects/windrag/
 * https://github.com/rohanrhu/WinDrag
 *
 * Copyright (C) 2017, Oğuzhan Eroğlu <rohanrhu2@gmail.com>
 * Licensed under MIT
 */

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WinDrag
{

    class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();
                
            bool is_first = true;
            bool is_mouse_pressed = false;
            bool is_moving = false;

            int rx = 0, ry = 0;
            int wx, wy;

            IntPtr w = IntPtr.Zero;
            IntPtr cw = IntPtr.Zero;

            RECT rect;

            while (true) {
                w = GetAncestor(WindowFromPoint(Cursor.Position), GA_ROOT);

                if (is_moving || (w != IntPtr.Zero))
                {
                    if (!is_mouse_pressed && (GetAsyncKeyState((UInt32)Keys.Menu) == -32768) && (GetAsyncKeyState(VK_LBUTTON) == -32768))
                    {
                        is_mouse_pressed = true;
                        is_moving = true;

                        cw = w;

                        rect = new RECT();
                        GetWindowRect(cw, ref rect);

                        rx = Cursor.Position.X - rect.Left;
                        ry = Cursor.Position.Y - rect.Top;

                        SetWindowLong(cw, GWL_EXSTYLE, WS_EX_LAYERED);
                        SetLayeredWindowAttributes(cw, 0, 200, LWA_ALPHA);
                    }

                    if ((GetAsyncKeyState(VK_LBUTTON) != -32768) || GetAsyncKeyState((UInt32)Keys.Menu) != -32768)
                    {
                        is_mouse_pressed = false;
                        is_moving = false;

                        SetLayeredWindowAttributes(cw, 0, 255, LWA_ALPHA);
                    }
                            
                    if (is_mouse_pressed && (GetAsyncKeyState((UInt32)Keys.Menu) == -32768) && (GetAsyncKeyState(VK_LBUTTON) == -32768))
                    {
                        wx = Cursor.Position.X - rx;
                        wy = Cursor.Position.Y - ry;

                        SetWindowPos(cw, IntPtr.Zero, wx, wy, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
                    }
                }
                else if ((GetAsyncKeyState(VK_LBUTTON) != -32767) && (GetAsyncKeyState(VK_LBUTTON) != -32768))
                {
                    is_mouse_pressed = false;
                    is_moving = false;

                    SetLayeredWindowAttributes(cw, 0, 255, LWA_ALPHA);
                }
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point loc);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOZORDER = 0x0004;

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);
        private const int GW_HWNDPREV = 3;

        [DllImport("user32.dll")]
        private static extern IntPtr GetAncestor(IntPtr hWnd, int gaFlags);
        private const int GA_ROOT = 2;

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(UInt32 vKey);
        private const UInt32 VK_LBUTTON = 0x01;

        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_LAYERED = 0x80000;
        public const int LWA_ALPHA = 0x2;
        public const int LWA_COLORKEY = 0x1;
    }
}
