using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HakeQuick.Abstraction.Action;
using HakeQuick.Abstraction.Services;
using HakeQuick.Implementation.Services.CancellationProvider;
using HakeQuick.Implementation.Base;

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
            if (x.Priority == y.Priority)
                return x.Action.Title.CompareTo(y.Action.Title);
            return x.Priority.CompareTo(y.Priority);
        }
    }

    internal static class SortedSetExtensions
    {
        public static int IndexOf<T>(this SortedSet<T> set, T item) where T : class
        {
            int index = 0;
            foreach (T i in set)
            {
                if (item == i)
                    return index;
                index++;
            }
            return -1;
        }
    }

    internal sealed class QuickContext : IInternalContext
    {
        private static Task COMPLETED_TASK { get; } = Task.Run(() => { });

        private List<AsyncActionUpdate> asyncActions = new List<AsyncActionUpdate>();

        private SortedSet<ActionWithPriorityWrapper> actions = new SortedSet<ActionWithPriorityWrapper>(new ActionWithPriorityWrapperComparer());
        public Dictionary<string, object> SharedData { get; } = new Dictionary<string, object>();
        public ICommand Command { get; }
        public ICancellationProvider CancellationProvider { get { return InternalCancellationProvider; } }
        public IInternalCancellationProvider InternalCancellationProvider { get; }

        public void AddAction(ActionBase action, ActionPriority priority = ActionPriority.Normal)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            actions.Add(new ActionWithPriorityWrapper(action, priority));
        }

        public void AddAsyncAction(AsyncActionUpdate update)
        {
            if (update == null)
                throw new ArgumentNullException(nameof(update));
            asyncActions.Add(update);
        }

        private object mutex_locker = new object();
        private ObservableCollection<ActionBase> target_list;
        private SynchronizationContext syncContext;
        public Task WaitResults(ObservableCollection<ActionBase> list)
        {
            target_list = list;
            syncContext = SynchronizationContext.Current;
            list.Clear();
            foreach (ActionWithPriorityWrapper wrapper in actions)
                list.Add(wrapper.Action);
            if (asyncActions.Count <= 0) return COMPLETED_TASK;

            return Task.Run(() =>
            {
                List<Task> waiterTasks = new List<Task>();
                Task waiterTask;
                foreach (AsyncActionUpdate update in asyncActions)
                {
                    if (update.UpdateTask.Status == TaskStatus.Created)
                        update.UpdateTask.Start();

                    if (update.UpdateTask.Status == TaskStatus.RanToCompletion)
                        ScheduleWaitingTask(update);
                    else if (update.UpdateTask.Status == TaskStatus.Running || update.UpdateTask.Status == TaskStatus.WaitingToRun || update.UpdateTask.Status == TaskStatus.WaitingForActivation)
                    {
                        waiterTask = new Task(ScheduleWaitingTask, update);
                        waiterTasks.Add(waiterTask);
                        waiterTask.Start();
                    }
                }
                Task.WaitAll(waiterTasks.ToArray());
            });
        }
        private void ScheduleWaitingTask(object state)
        {
            AsyncActionUpdate update = state as AsyncActionUpdate;
            ActionWithPriorityWrapper placeholder = null;
            if (update.UpdateTask.Status == TaskStatus.Running || update.UpdateTask.Status == TaskStatus.WaitingToRun || update.UpdateTask.Status == TaskStatus.WaitingForActivation)
            {
                placeholder = new ActionWithPriorityWrapper(update.Placeholder, update.Priority);
                lock (mutex_locker)
                {
                    actions.Add(placeholder);
                    int index = actions.IndexOf(placeholder);
                    syncContext.Send(s => target_list.Insert(index, placeholder.Action), null);
                }
                try { update.UpdateTask.Wait(); }
                catch (AggregateException ex)
                {
                    if (ex.InnerException is TaskCanceledException) { }
                    else
                        throw ex;
                }
            }
            if (update.UpdateTask.Status == TaskStatus.RanToCompletion)
            {
                if (placeholder != null)
                {
                    lock (mutex_locker)
                    {
                        syncContext.Send(s => target_list.Remove(placeholder.Action), null);
                        actions.Remove(placeholder);
                    }
                }
                ActionUpdateResult result = update.UpdateTask.Result;
                ActionWithPriorityWrapper wrapper = new ActionWithPriorityWrapper(result.Action, result.Priority);
                lock (mutex_locker)
                {
                    actions.Add(wrapper);
                    int index = actions.IndexOf(wrapper);
                    syncContext.Send(s => target_list.Insert(index, wrapper.Action), null);
                }
                return;
            }
            else if (placeholder != null && (update.UpdateTask.Status == TaskStatus.Canceled || update.UpdateTask.Status == TaskStatus.Faulted))
            {
                lock (mutex_locker)
                {
                    syncContext.Send(s => target_list.Remove(placeholder.Action), null);
                    actions.Remove(placeholder);
                }
            }
        }

        private bool disposed = false;
        ~QuickContext()
        {
            if (!disposed) Dispose();
        }
        public void Dispose()
        {
            if (disposed) return;
            InternalCancellationProvider.Dispose();
            disposed = true;
        }

        public QuickContext(ICommand command)
        {
            Command = command;
            InternalCancellationProvider = new CancellationProvider();
        }
    }
}
