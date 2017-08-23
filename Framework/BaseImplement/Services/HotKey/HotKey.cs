using HakeQuick.Abstraction.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace HakeQuick.Implementation.Services.HotKey
{
    internal sealed class HotKey : IHotKey, IMessageFilter
    {
        #region NativeMethods
        [DllImport("user32.dll")]
        private static extern uint RegisterHotKey(IntPtr hwnd, uint id, uint fsmod, uint virtualkey);
        [DllImport("user32.dll")]
        private static extern uint UnregisterHotKey(IntPtr hwnd, uint id);
        [DllImport("kernel32.dll")]
        private static extern uint GlobalAddAtom(string lpstring);
        [DllImport("kernel32.dll")]
        private static extern uint GlobalDeleteAtom(uint nAtom);
        #endregion NativeMethods

        private Key bindedKey;
        private KeyFlags bindedFlags;
        private uint atomID = 0;

        public bool KeyBinded { get { return atomID > 0; } }
        public event HotKeyEventHandler KeyPressed;

        public HotKey(Key key, KeyFlags flags)
        {
            bindedKey = key;
            bindedFlags = flags;
        }

        public void BindKey()
        {
            if (KeyBinded)
                throw new InvalidOperationException("unbind must be called when already binded to a hotkey");

            uint hotkeyid = GlobalAddAtom(Guid.NewGuid().ToString());
            int keycode = KeyInterop.VirtualKeyFromKey(bindedKey);
            RegisterHotKey(IntPtr.Zero, hotkeyid, (uint)bindedFlags, (uint)keycode);
            atomID = hotkeyid;
            Application.AddMessageFilter(this);
        }
        public void UnbindKey()
        {
            if (KeyBinded == false)
                return;

            UnregisterHotKey(IntPtr.Zero, atomID);
            GlobalDeleteAtom(atomID);
            atomID = 0;
            Application.RemoveMessageFilter(this);
        }


        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == 0x312)
            {
                KeyPressed?.Invoke(this, m.WParam.ToInt32());
            }
            return false;
        }

        private bool disposed = false;
        ~HotKey()
        {
            if (!disposed)
                Dispose();
        }
        public void Dispose()
        {
            if (disposed == true)
                return;
            UnbindKey();
            disposed = true;
        }
    }
}
