namespace HakeQuick.Abstraction.Action
{
    public sealed class ActionUpdateResult
    {
        public ActionBase Action { get; }
        public ActionPriority Priority { get; }

        public ActionUpdateResult(ActionBase action, ActionPriority priority)
        {
            Action = action;
            Priority = priority;
        }
    }
}
