using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HakeQuick.Implementation.Services.HotKey
{
    public delegate void HotKeyEventHandler(IHotKey sender, int hotkeyid);

    public interface IHotKey : IDisposable
    {
        event HotKeyEventHandler KeyPressed;

        void BindKey();
        void UnbindKey();

    }
}
