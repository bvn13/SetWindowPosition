using System;

using HWND = System.IntPtr;

namespace SetWindowPosition {
    public class WindowInfo {
        public HWND hWnd;
        public string name;
        public string title;

        public WindowInfo(HWND hWnd, string name, string title) {
            this.hWnd = hWnd;
            this.name = name;
            this.title = title;
        }

        public override string ToString() {
            return "" + hWnd.ToString() + " | " + name + " | " + title.ToString();
        }
    }
}
