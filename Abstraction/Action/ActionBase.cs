using HakeQuick.Abstraction.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace HakeQuick.Abstraction.Action
{
    public enum ActionPriority
    {
        Topmost = 0,
        VeryHigh = 1,
        High = 2,
        Normal = 3,
        Low = 4,
        VeryLow = 5
    }

    public abstract class ActionBase : ViewModelBase
    {
        private BitmapImage _icon;
        public BitmapImage Icon
        {
            get { return _icon; }
            protected set { _icon = value; NotifyPropertyChanged(nameof(Icon)); }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            protected set { _title = value; NotifyPropertyChanged(nameof(Title)); }
        }

        private string _subtitle;
        public string Subtitle
        {
            get { return _subtitle; }
            protected set { _subtitle = value; NotifyPropertyChanged(nameof(Subtitle)); }
        }

        private bool _executable;
        public bool IsExecutable
        {
            get { return _executable; }
            protected set { _executable = value; NotifyPropertyChanged(nameof(IsExecutable)); }
        }

        // Create member called Invoke
    }

    public sealed class PlaceholderAction : ActionBase
    {
        public PlaceholderAction(BitmapImage icon, string title, string subtitle)
        {
            IsExecutable = false;
            Icon = icon;
            Title = title;
            Subtitle = subtitle;
        }
    }

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

    public sealed class AsyncActionUpdate
    {
        public Task<ActionUpdateResult> UpdateTask { get; }
        public PlaceholderAction Placeholder { get; }
        public ActionPriority Priority { get; }

        private AsyncActionUpdate(PlaceholderAction placeholder, ActionPriority priority, Func<ActionUpdateResult> updateTask)
        {
            if (placeholder == null)
                throw new ArgumentNullException(nameof(placeholder));

            Priority = priority;
            Placeholder = placeholder;
            UpdateTask = new Task<ActionUpdateResult>(updateTask);
            UpdateTask.Start();
        }

        public static AsyncActionUpdate Create(PlaceholderAction placeholder, ActionPriority priority, Func<ActionUpdateResult> task)
        {
            return new AsyncActionUpdate(placeholder, priority, task);
        }
        public static AsyncActionUpdate Create(PlaceholderAction placeholder, ActionPriority priority, Func<Task<ActionUpdateResult>> task)
        {
            return new AsyncActionUpdate(placeholder, priority, ()=>
            {
                Task<ActionUpdateResult> tsk = task();
                tsk.Wait();
                return tsk.Result;
            });
        }
    }
}
