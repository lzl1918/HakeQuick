using System.Collections.Generic;

using Hake.Extension.Pipeline.Abstraction;
using HakeQuick.Abstraction.Action;

namespace HakeQuick.Abstraction.Base
{
    public interface IQuickContext : IContext
    {
        ICommand Command { get; }

        void AddAction(ActionBase action, ActionPriority priority = ActionPriority.Normal);

        IEnumerable<ActionBase> RetriveActions();
    }
}
