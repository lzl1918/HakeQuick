using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HakeQuick.Abstraction.Services
{
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
    public interface IProgramContext
    {
        RECT WindowPosition { get; }
        Process CurrentProcess { get; }
        IntPtr WindowHandle { get; }
        IntPtr DesktopHandle { get; }

        int ThreadId { get; }
        int ProcessId { get; }
        bool IsDesktop { get; }
    }
}
