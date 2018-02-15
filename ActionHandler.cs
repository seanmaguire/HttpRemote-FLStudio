using System;
using System.Text;
using System.Diagnostics;


namespace HttpRemote
{
    public class ActionHandler
    {

        private static string audacityLocation = @"C:\Program Files (x86)\Audacity\audacity.exe";


        public static void parsePost(string data)
        {
            if (data.StartsWith("flpedal"))
            {
                int n = Convert.ToInt32(data.Split(' ')[1]);
                int v = Convert.ToInt32(data.Split(' ')[2]);

                Console.WriteLine("click thing " + n + " " + v);
                clickFLPedal("Pedals", n, v);
            }
            else if (data.StartsWith("audacity"))
            {
                int n = Convert.ToInt32(data.Split(' ')[1]);
                int v = Convert.ToInt32(data.Split(' ')[2]);

                System.Console.WriteLine(n + " " + v);

                if (n == 1 && v == 0) audacityRecordNew();
                if (n == 1 && v == 1) audacityClickStop();

                //winAPI.MoveTo(x, y);
            }
            else if (data.StartsWith("swipe"))
            {
                int x = Convert.ToInt32(data.Split(' ')[1]);
                int y = Convert.ToInt32(data.Split(' ')[2]);

                WinAPI.SetCursorPos(x, y);
                //WinAPI.MoveTo(x, y);
            }
            else if (data.StartsWith("tap"))
            {
                WinAPI.POINT p;
                WinAPI.GetCursorPos(out p);
                WinAPI.LeftMouseClick(p.X, p.Y);
            }
        }


        public static void audacityRecordNew()
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.FileName = audacityLocation;
            process.StartInfo = startInfo;
            process.Start();

            System.Threading.Thread.Sleep(2000);

            IntPtr hWnd = WinAPI.GetForegroundWindow();

            WinAPI.RECT rect;
            WinAPI.GetWindowRect(hWnd, out rect);
            Console.WriteLine("(" + rect.Top + "," + rect.Left + "," + rect.Bottom + "," + rect.Right + ")");

            WinAPI.LeftMouseClick(rect.Top + 320, rect.Left + 65);
        }


        public static void audacityClickStop()
        {
            IntPtr hWnd = WinAPI.GetForegroundWindow();

            WinAPI.RECT rect;
            WinAPI.GetWindowRect(hWnd, out rect);
            Console.WriteLine("(" + rect.Top + "," + rect.Left + "," + rect.Bottom + "," + rect.Right + ")");

            WinAPI.LeftMouseClick(rect.Top + 170, rect.Left + 65);
        }
        

        public static void clickFLPedal(string name, int n, int v)
        {
            foreach (Process procesInfo in Process.GetProcesses())
            {
                if (!procesInfo.ProcessName.Equals("FL")) continue;

                //Console.WriteLine("process {0} {1}", procesInfo.ProcessName, procesInfo.Id);
                foreach (ProcessThread threadInfo in procesInfo.Threads)
                {
                    //Console.WriteLine("\tthread {0:x}", threadInfo.Id);
                    IntPtr[] windows = WinAPI.GetWindowHandlesForThread(threadInfo.Id);
                    if (windows != null && windows.Length > 0)
                    {
                        foreach (IntPtr hWnd in windows)
                        {
                            //Console.Write("\twindow {0} text:{1} caption:{2} rect: ",
                            //    hWnd.ToInt32(), GetText(hWnd), GetEditText(hWnd));

                            WinAPI.RECT rect;
                            WinAPI.GetWindowRect(hWnd, out rect);
                            //Console.WriteLine("(" + rect.Top + "," + rect.Left + "," + rect.Bottom + "," + rect.Right + ")");

                            if (WinAPI.GetText(hWnd).Contains(name))
                            {
                                if (rect.Left > 0)
                                {
                                    WinAPI.POINT lpPoint;
                                    WinAPI.GetCursorPos(out lpPoint);

                                    int dist = (n - 1) * 166;

                                    // first pedal
                                    WinAPI.LeftMouseClick(rect.Left + 87 + dist, rect.Top + 351);

                                    WinAPI.SetCursorPos(lpPoint.X, lpPoint.Y);
                                }
                                return;
                            }
                        }
                    }
                }
            }
            /*
            POINT lpPoint;
            GetCursorPos(out lpPoint);
            System.Console.WriteLine("\n\n\n" + lpPoint.X + " " + lpPoint.Y + "\n\n\n");

            RECT rect;
            IntPtr hWnd = new IntPtr(133578);
            GetWindowRect(hWnd, out rect);
            System.Console.WriteLine("\n" + rect.Left + " " + rect.Top + "\n" + rect.Right + " " + rect.Bottom + "\n");
            */
        }

    }
}
