using HakeQuick.Abstraction.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HakeQuick.Implementation.Base
{
    internal interface IInternalCancellationProvider : ICancellationProvider, IDisposable
    {
        void Cancel();
    }
}
