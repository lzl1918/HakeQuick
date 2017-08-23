using HakeQuick.Abstraction.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HakeQuick.Implementation.Services.HotKey
{
    internal sealed class HotKeyBuilder : IHotKeyBuilder
    {
        private Key bindedKey;
        private KeyFlags bindedFlags;
        private bool canBuild = false;
        public IHotKey Build()
        {
            if (!canBuild)
                throw new InvalidOperationException("cannot build hotkey");
            return new HotKey(bindedKey, bindedFlags);
        }

        public void SetBinding(Key key, KeyFlags flags)
        {
            bindedKey = key;
            bindedFlags = flags;
            canBuild = true;
        }
    }
}
