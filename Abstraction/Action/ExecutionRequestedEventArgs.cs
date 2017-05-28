using System;

namespace HakeQuick.Abstraction.Action
{
    public sealed class ExecutionRequestedEventArgs : EventArgs
    {
        public ActionBase Action { get; }

        public ExecutionRequestedEventArgs(ActionBase action)
        {
            Action = action;
        }
    }
}
