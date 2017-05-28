using System.Collections.Generic;
using HakeQuick.Abstraction.Action;

namespace HakeQuick.Abstraction.Base
{
    public static class QuickContextExtension
    {
        public static void AddActions(this IQuickContext context, IEnumerable<ActionBase> actions, ActionPriority priority = ActionPriority.Normal)
        {
            foreach (ActionBase action in actions)
                context.AddAction(action, priority);
        }
    }
}
