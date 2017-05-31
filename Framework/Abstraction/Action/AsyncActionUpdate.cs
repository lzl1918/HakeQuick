using System;
using System.Threading.Tasks;

namespace HakeQuick.Abstraction.Action
{
    public sealed class AsyncActionUpdate
    {
        public Task<ActionUpdateResult> UpdateTask { get; }
        public PlaceholderAction Placeholder { get; }
        public ActionPriority Priority { get; }

        private AsyncActionUpdate(PlaceholderAction placeholder, ActionPriority priority, Task<ActionUpdateResult> updateTask)
        {
            if (placeholder == null)
                throw new ArgumentNullException(nameof(placeholder));

            Priority = priority;
            Placeholder = placeholder;
            UpdateTask = updateTask;
            if (UpdateTask.Status == TaskStatus.Created)
                UpdateTask.Start();
        }

        public static AsyncActionUpdate Create(PlaceholderAction placeholder, ActionPriority priority, Func<ActionUpdateResult> task)
        {
            return new AsyncActionUpdate(placeholder, priority,
                Task.Run<ActionUpdateResult>(() =>
                {
                    return task();
                })
            );
        }
        public static AsyncActionUpdate Create(PlaceholderAction placeholder, ActionPriority priority, Func<Task<ActionUpdateResult>> task)
        {
            return new AsyncActionUpdate(placeholder, priority, task());
        }
    }
}
