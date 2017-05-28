using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HakeQuick.Abstraction.Action;

namespace HakeQuick.Abstraction.Base
{
    internal sealed class ActionWithPriorityWrapper
    {
        public ActionBase Action { get; set; }
        public ActionPriority Priority { get; set; }
        public ActionWithPriorityWrapper(ActionBase action, ActionPriority priority)
        {
            Action = action;
            Priority = priority;
        }
    }
    internal sealed class ActionWithPriorityWrapperComparer : IComparer<ActionWithPriorityWrapper>
    {
        public int Compare(ActionWithPriorityWrapper x, ActionWithPriorityWrapper y)
        {
            return x.Priority.CompareTo(y.Priority);
        }
    }

    public sealed class QuickContext : IQuickContext
    {
        private SortedSet<ActionWithPriorityWrapper> actions = new SortedSet<ActionWithPriorityWrapper>(new ActionWithPriorityWrapperComparer());
        public ICommand Command { get; }

        public IEnumerable<ActionBase> RetriveActions()
        {
            return actions.Select(item => item.Action);
        }

        public void AddAction(ActionBase action, ActionPriority priority = ActionPriority.Normal)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            actions.Add(new ActionWithPriorityWrapper(action, priority));
        }
        
        public QuickContext(ICommand command)
        {
            Command = command;
        }
    }
}
