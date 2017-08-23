using HakeQuick.Abstraction.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HakeQuick.Implementation.Services.HotKey
{
    public interface IHotKeyBuilder
    {
        void SetBinding(Key key, KeyFlags flags);
        IHotKey Build();
    }
}
