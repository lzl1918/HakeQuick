using HakeQuick.Abstraction.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace HakeQuick.Implementation.Services.ProgramContext
{
    internal sealed class ProgramContext : IProgramContext
    {
        private static class Win32
        {
            [DllImport("user32.dll")]
            public static extern int GetWindowRect(IntPtr hwnd, out RECT lpRect);
            [DllImport("user32.dll")]
            public static extern IntPtr GetForegroundWindow();
            [DllImport("user32.dll")]
            public static extern bool SetForegroundWindow(IntPtr ptr);
            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);
            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();
        }


        public Process CurrentProcess { get; }
        public IntPtr WindowHandler { get; }
        public IntPtr DesktopHandler { get; }
        public int ThreadId { get; }
        public int ProcessId { get; }
        public bool IsDesktop { get { return WindowHandler == DesktopHandler; } }
        public RECT WindowPosition { get; }

        public ProgramContext()
        {
            int pid;
            WindowHandler = Win32.GetForegroundWindow();
            ThreadId = Win32.GetWindowThreadProcessId(WindowHandler, out pid);
            ProcessId = pid;
            CurrentProcess = Process.GetProcessById(pid);
            DesktopHandler = Win32.GetDesktopWindow();
            RECT rect;
            Win32.GetWindowRect(WindowHandler, out rect);
            WindowPosition = rect;
        }
    }
}
