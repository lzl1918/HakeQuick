using HakeQuick.Abstraction.Action;
using HakeQuick.Abstraction.Services;
using HakeQuick.Helpers;
using System;
using System.Diagnostics;
using System.Reflection;

namespace RunnerPlugin
{
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

        public void Invoke(IProgramContext progContext, ILogger runnerLogger, bool admin = false)
        {
            ProcessStartInfo psi = new ProcessStartInfo(exeCommand, exeArgs);
            psi.WorkingDirectory = Helper.CurrentWorkingDirectoryOrDefault(progContext);
            if (admin)
            {
                psi.Verb = "runas";
            }
            try
            {
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                runnerLogger.LogExceptionAsync(ex);
            }
        }
    }
}
