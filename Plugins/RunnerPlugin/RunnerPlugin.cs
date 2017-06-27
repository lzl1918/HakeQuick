using HakeQuick.Abstraction.Action;
using HakeQuick.Abstraction.Base;
using HakeQuick.Abstraction.Plugin;
using HakeQuick.Abstraction.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RunnerPlugin
{
    internal static class WinAPI
    {
    }
    public sealed class RunnerPlugin : QuickPlugin
    {
        private RunnerAction runner = new RunnerAction();

        public RunnerPlugin(ILoggerFactory loggerFactory)
        {
            loggerFactory.CreateLogger("Runner");
        }

        [IgnoreIdentity("run")]
        public ActionUpdateResult OnUpdate(ICommand command, bool admin = false, string action = "")
        {
            StringBuilder argsBuilder = new StringBuilder(command.Raw.Length);
            foreach (object obj in command.UnnamedArguments)
            {
                if (obj is string str)
                {
                    argsBuilder.Append(str);
                    argsBuilder.Append(' ');
                }
            }
            action = action.Trim();
            string args = argsBuilder.ToString();
            if (action.Length > 0)
            {
                runner.Update($"运行 - {action}", $"{action} {args}");
                runner.UpdateCommand(action, args);
            }
            else
            {
                runner.Update("运行", "运行新任务");
            }
            return new ActionUpdateResult(runner, ActionPriority.High);
        }
    }
}
