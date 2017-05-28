using System;

namespace HakeQuick.Implementation.Services.TerminationNotifier
{
    internal sealed class TerminationNotifier : ITerminationNotifier
    {
        public event EventHandler TerminationNotified;

        public void NotifyTerminate()
        {
            TerminationNotified?.Invoke(this, null);
        }
    }
}
