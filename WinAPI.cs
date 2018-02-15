using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace HttpRemote
{
    public static class WinAPI
    {
        // windows
        [DllImport("User32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int index);

        [DllImport("User32.dll")]
        private static extern IntPtr SendDlgItemMessage(IntPtr hWnd, int IDDlgItem, int uMsg, int nMaxCount, StringBuilder lpString);

        [DllImport("User32.dll")]
        private static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        private delegate int EnumWindowsProc(IntPtr hwnd, int lParam);

        [DllImport("user32.dll")]
        private static extern int EnumWindows(EnumWindowsProc x, int y);

        [DllImport("user32")]
        private static extern bool EnumChildWindows(IntPtr window, EnumWindowsProc callback, int lParam);

        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        private static List<IntPtr> _results = new List<IntPtr>();


        // get window text / size
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);


        // mouse
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        private static extern int GetSystemMetrics(int index);

        const int MOUSEEVENTF_MOVE = 0x0001; /* mouse move */
        const int MOUSEEVENTF_LEFTDOWN = 0x0002; /* left button down */
        const int MOUSEEVENTF_LEFTUP = 0x0004; /* left button up */
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008; /* right button down */
        const int MOUSEEVENTF_RIGHTUP = 0x0010; /* right button up */
        const int MOUSEEVENTF_MIDDLEDOWN = 0x0020; /* middle button down */
        const int MOUSEEVENTF_MIDDLEUP = 0x0040; /* middle button up */
        const int MOUSEEVENTF_XDOWN = 0x0080; /* x button down */
        const int MOUSEEVENTF_XUP = 0x0100; /* x button down */
        const int MOUSEEVENTF_WHEEL = 0x0800; /* wheel button rolled */
        const int MOUSEEVENTF_ABSOLUTE = 0x8000; /* absolute move */

        const int SM_CXSCREEN = 0;
        const int SM_CYSCREEN = 1;

        private static int Delay = 0;

        private static void DoEvent(int eventType, int globalX, int globalY)
        {
            globalX = globalX > 0 ? globalX : 0;
            globalY = globalY > 0 ? globalY : 0;
            int x = Convert.ToInt32((float)globalX * 65536 / GetSystemMetrics(SM_CXSCREEN));
            int y = Convert.ToInt32((float)globalY * 65536 / GetSystemMetrics(SM_CYSCREEN));
            mouse_event(MOUSEEVENTF_ABSOLUTE | eventType, x, y, 0, 0);
            System.Threading.Thread.Sleep(Delay);
            Application.DoEvents();
        }

        public static void LeftDown(int globalX, int globalY)
        {
            DoEvent(MOUSEEVENTF_MOVE, globalX, globalY);
            DoEvent(MOUSEEVENTF_LEFTDOWN, globalX, globalY);
        }

        public static void LeftUp(int globalX, int globalY)
        {
            DoEvent(MOUSEEVENTF_MOVE, globalX, globalY);
            DoEvent(MOUSEEVENTF_LEFTUP, globalX, globalY);
        }

        public static void RightDown(int globalX, int globalY)
        {
            DoEvent(MOUSEEVENTF_MOVE, globalX, globalY);
            DoEvent(MOUSEEVENTF_RIGHTDOWN, globalX, globalY);
        }

        public static void RightUp(int globalX, int globalY)
        {
            DoEvent(MOUSEEVENTF_MOVE, globalX, globalY);
            DoEvent(MOUSEEVENTF_RIGHTUP, globalX, globalY);
        }

        //This simulates a left mouse click
        public static void LeftMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            LeftDown(xpos, ypos);
            LeftUp(xpos, ypos);
        }

        public struct POINT
        {
            public int X;
            public int Y;

            /*public static implicit operator POINT(POINT point)
            {
                return new POINT(point.X, point.Y);
            }*/
        }

        public static void MoveMouseTo(int globalX, int globalY)
        {
            MoveMouseTo(globalX, globalY, 0);
        }

        public static void MoveMouseTo(int globalX, int globalY, int speed)
        {
            if (speed != 0)
            {
                int cursorX = Cursor.Position.X;
                int cursorY = Cursor.Position.Y;
                int dX = globalX - cursorX;
                int dY = globalY - cursorY;
                int steps = Math.Max(Math.Abs(dX / speed), Math.Abs(dY / speed));
                if (steps != 0)
                {
                    int stepX = dX / steps;
                    int stepY = dY / steps;
                    for (int i = 0; i < steps; i++)
                    {
                        cursorX += stepX;
                        cursorY += stepY;
                        DoEvent(MOUSEEVENTF_MOVE, cursorX, cursorY);
                    }
                }
            }
            DoEvent(MOUSEEVENTF_MOVE, globalX, globalY);
        }

        public static void DragAndDrop(int startX, int startY, int finishX, int finishY)
        {
            LeftDown(startX, startY);
            MoveMouseTo(finishX, finishY, 10);
            LeftUp(finishX, finishY);
        }

        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public static string GetText(IntPtr hWnd)
        {
            int length = GetWindowTextLength(hWnd);
            StringBuilder sb = new StringBuilder(length + 1);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        // get richedit text
        private const int GWL_ID = -12;
        private const int WM_GETTEXT = 0x000D;


        public static IntPtr[] GetWindowHandlesForThread(int threadHandle)
        {
            _results.Clear();
            EnumWindows(WindowEnum, threadHandle);
            return _results.ToArray();
        }

        public static int WindowEnum(IntPtr hWnd, int lParam)
        {
            int processID = 0;
            int threadID = GetWindowThreadProcessId(hWnd, out processID);
            if (threadID == lParam)
            {
                _results.Add(hWnd);
                EnumChildWindows(hWnd, WindowEnum, threadID);
            }
            return 1;
        }

        public static StringBuilder GetEditText(IntPtr hWnd)
        {
            Int32 dwID = GetWindowLong(hWnd, GWL_ID);
            IntPtr hWndParent = GetParent(hWnd);
            StringBuilder title = new StringBuilder(128);
            SendDlgItemMessage(hWndParent, dwID, WM_GETTEXT, 128, title);
            return title;
        }

    }

}

