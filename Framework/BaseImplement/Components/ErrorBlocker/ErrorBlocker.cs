using HakeQuick.Abstraction.Action;
using HakeQuick.Abstraction.Base;
using HakeQuick.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace HakeQuick.Implementation.Components.ErrorBlocker
{
    internal sealed class UpdatableErrorAction : ErrorAction
    {
        public UpdatableErrorAction(BitmapImage icon, string title, string subtitle) : base(icon, title, subtitle)
        {
        }

        public void Set(BitmapImage icon, string title, string subtitle)
        {
            Icon = icon;
            Title = title;
            Subtitle = subtitle;
        }
        public void Set(string title, string subtitle)
        {
            Title = title;
            Subtitle = subtitle;
        }
    }
    internal sealed class ErrorBlocker
    {
        private static Task completedTask = Task.Run(() => { });
        private bool blockIfError;
        private static UpdatableErrorAction errorAction;
        public ErrorBlocker(bool blockIfError)
        {
            this.blockIfError = blockIfError;
            if (errorAction == null)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                BitmapImage erroricon = assembly.LoadImage("BaseImplement.Resources.run.png");
                errorAction = new UpdatableErrorAction(erroricon, "", "");
            }
        }

        public Task Invoke(IQuickContext context, Func<Task> next)
        {
            ICommand command = context.Command;
            if (!command.ContainsError)
                return next();

            errorAction.Set("ERORR", command.Raw);
            context.AddAction(errorAction, ActionPriority.Topmost);
            if (blockIfError)
                return completedTask;
            else
                return next();
        }
    }
}
