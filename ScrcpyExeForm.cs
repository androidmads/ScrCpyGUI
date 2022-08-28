using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ScrCpyGUI
{
    public partial class ScrcpyExeForm : Form
    {
        System.Diagnostics.Process Process;
        string Command;

        public ScrcpyExeForm(System.Diagnostics.Process process, string command)
        {
            InitializeComponent();
            Process = process;
            Command = command;
            //SetParent(Process.MainWindowHandle, panel1.Handle);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Process.Start();
            Process.WaitForExit();
            //var app = System.Diagnostics.Process.Start(Command);
            SetParent(Process.MainWindowHandle, panel1.Handle);
            /*  SetWindowLongPtr(new HandleRef(null, Process.MainWindowHandle), Constants.GWL_STYLE, new IntPtr(Constants.WS_VISIBLE));
             MoveWindow(Process.MainWindowHandle, 0, 0, (int)450, (int)600, true);*/
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);

        // This static method is required because legacy OSes do not support
        // SetWindowLongPtr
        public static IntPtr SetWindowLongPtr(HandleRef hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            else
                return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(HandleRef hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, IntPtr dwNewLong);
    }

    internal class Constants
    {
        public const int GWL_STYLE = -16;

        public const int WS_VISIBLE = 0x10000000;
    }

}
