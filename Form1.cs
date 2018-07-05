using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Runtime.InteropServices;

using HWND = System.IntPtr;

namespace SetWindowPosition
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            preloadProcesses();
        }

        // Define the FindWindow API function.
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        // Define the SetWindowPos API function.
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetShellWindow();

        private delegate bool EnumWindowsProc(HWND hWnd, int lParam);

        [DllImport("USER32.DLL")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("USER32.DLL")]
        private static extern bool IsWindowVisible(HWND hWnd);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowTextLength(HWND hWnd);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowText(HWND hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32")]
        private static extern UInt32 GetWindowThreadProcessId(Int32 hWnd, out Int32 lpdwProcessId);

        public static Int32 GetWindowProcessID(Int32 hwnd) {
            Int32 pid = 1;
            GetWindowThreadProcessId(hwnd, out pid);
            return pid;
        }

        // Define the SetWindowPosFlags enumeration.
        [Flags()]
        private enum SetWindowPosFlags : uint
        {
            SynchronousWindowPosition = 0x4000,
            DeferErase = 0x2000,
            DrawFrame = 0x0020,
            FrameChanged = 0x0020,
            HideWindow = 0x0080,
            DoNotActivate = 0x0010,
            DoNotCopyBits = 0x0100,
            IgnoreMove = 0x0002,
            DoNotChangeOwnerZOrder = 0x0200,
            DoNotRedraw = 0x0008,
            DoNotReposition = 0x0200,
            DoNotSendChangingEvent = 0x0400,
            IgnoreResize = 0x0001,
            IgnoreZOrder = 0x0004,
            ShowWindow = 0x0040,
        }

        /// <summary>Returns a dictionary that contains the handle and title of all the open windows.</summary>
        /// <returns>A dictionary that contains the handle and title of all the open windows.</returns>
        public static HashSet<WindowInfo> GetOpenWindows() {
            HWND shellWindow = GetShellWindow();
            
            HashSet<WindowInfo> windows = new HashSet<WindowInfo>();

            EnumWindows(delegate (HWND hWnd, int lParam)
            {
                if (hWnd == shellWindow) return true;
                if (!IsWindowVisible(hWnd)) return true;

                int length = GetWindowTextLength(hWnd);
                if (length == 0) return true;

                StringBuilder builder = new StringBuilder(length);
                GetWindowText(hWnd, builder, length + 1);

                Int32 pid = GetWindowProcessID(hWnd.ToInt32());
                Process p = Process.GetProcessById(pid);
                string appName = p.ProcessName;

                WindowInfo window = new WindowInfo(hWnd, appName, builder.ToString());

                windows.Add(window);
                return true;

            }, 0);

            return windows;
        }

        private void preloadProcesses() {
            HashSet<WindowInfo> allWindows = GetOpenWindows();
            foreach (WindowInfo windowData in allWindows) {
                comboBox1.Items.Add(windowData);
            }
        }

        // Size and position the application.
        private void btnSet_Click(object sender, EventArgs e)
        {

            // Get the target window's handle.
            IntPtr target_hwnd = //FindWindowByCaption(IntPtr.Zero, txtAppTitle.Text);
                ((WindowInfo) comboBox1.SelectedItem).hWnd;
            if (target_hwnd == IntPtr.Zero)
            {
                MessageBox.Show(
                    "Could not find a window with the title \"" +
                    ((WindowInfo) comboBox1.SelectedItem).title+ "\"");
                return;
            }
            

            // Set the window's position.
            int width = int.Parse(txtWidth.Text);
            int height = int.Parse(txtHeight.Text);
            int x = int.Parse(txtX.Text);
            int y = int.Parse(txtY.Text);
            SetWindowPos(target_hwnd, IntPtr.Zero, x, y, width, height, 0);
        }

        
    }
}
