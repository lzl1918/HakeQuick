using HakeQuick.Abstraction.Action;
using HakeQuick.Abstraction.Base;
using HakeQuick.Abstraction.Plugin;
using HakeQuick.Abstraction.Services;
using HakeQuick.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RunnerPlugin
{
    internal static class WinAPI
    {
    }

    internal sealed class RunnerAction : ModifiableActionBase
    {
        private string exeCommand = "";
        private string exeArgs = "";

        public RunnerAction()
        {
            Icon = Assembly.GetExecutingAssembly().LoadImage("RunnerPlugin.Resources.run.png");
            Title = "运行";
            Subtitle = "运行新任务";
            IsExecutable = true;
        }

        public void UpdateCommand(string command, string args)
        {
            exeCommand = command;
            exeArgs = args;
        }

        public void Invoke(IProgramContext progContext, bool admin = false)
        {
            ProcessStartInfo psi = new ProcessStartInfo(exeCommand, exeArgs);
            psi.WorkingDirectory = Helper.CurrentWorkingDirectoryOrDefault(progContext);
            if(admin)
            {
                psi.Verb = "runas";
            }
            try
            {
                Process.Start(psi);
            }
            catch
            {
            }
        }
    }
    public sealed class RunnerPlugin : QuickPlugin
    {
        private RunnerAction runner = new RunnerAction();

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
