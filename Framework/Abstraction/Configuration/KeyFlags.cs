using System;

namespace HakeQuick.Abstraction.Base
{
    [Flags]
    public enum KeyFlags : uint
    {
        None = 0x0,
        Alt = 0x1,
        Control = 0x2,
        Shift = 0x4,
        Win = 0x8,
        NoRepeat = 0x4000,
    }
}
