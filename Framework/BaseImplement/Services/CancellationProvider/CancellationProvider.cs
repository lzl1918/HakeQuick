using HakeQuick.Abstraction.Services;
using HakeQuick.Implementation.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HakeQuick.Implementation.Services.CancellationProvider
{
    internal sealed class CancellationProvider : IInternalCancellationProvider
    {
        public CancellationToken CancellationToken { get { return source.Token; } }

        public bool IsCancellationRequested { get { return source.IsCancellationRequested; } }

        private CancellationTokenSource source;
        public CancellationProvider()
        {
            source = new CancellationTokenSource();
        }


        public void Cancel()
        {
            source.Cancel();
        }

        private bool disposed = false;
        ~CancellationProvider()
        {
            if (!disposed) Dispose();
        }
        public void Dispose()
        {
            if (disposed) return;

            source.Dispose();
            disposed = true;
        }
    }
}
