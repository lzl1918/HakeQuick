using HakeQuick.Abstraction.Action;
using HakeQuick.Abstraction.Base;
using HakeQuick.Abstraction.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Media.Imaging;

namespace RunnerPlugin
{
    internal sealed class RunCommandAction : ActionBase
    {
        private bool defaultAsAdmin;
        private string workingDirectory;
        private string arguments;
        public string RunCommand { get; }
        private string commandPath;
        public RunCommandAction(string run, string path, string iconPath, bool admin, string workingDirectory, IList<string> args)
        {
            defaultAsAdmin = admin;
            this.workingDirectory = workingDirectory;
            if (args == null)
                arguments = null;
            else
            {
                StringBuilder argBuilder = new StringBuilder();
                foreach (string arg in args)
                {
                    argBuilder.Append(arg);
                    argBuilder.Append(' ');
                }
                if (argBuilder.Length > 0)
                    argBuilder.Remove(argBuilder.Length - 1, 1);
                arguments = argBuilder.ToString();
            }
            string appendArgs = arguments == null ? "" : " " + arguments;
            path = path ?? "";
            path = path.Trim();
            commandPath = path;
            IsExecutable = true;
            Title = "运行 - " + run;
            if (path.Length > 0)
                Subtitle = path + appendArgs;
            else
                Subtitle = run + appendArgs;
            RunCommand = run.ToLower();
            if (iconPath != null)
            {
                try
                {
                    Icon = new BitmapImage(new Uri(iconPath));
                }
                catch
                {
                    Icon = null;
                }
            }
        }

        public void Invoke(IProgramContext progContext, ILogger runnerLogger, ICommand command, bool admin = false, bool a = false)
        {
            admin = admin || a || defaultAsAdmin;
            string procname = RunCommand;
            if (commandPath.Length > 0)
                procname = commandPath;
            ProcessStartInfo psi = new ProcessStartInfo(procname);
            if (arguments != null)
                psi.Arguments = arguments;

            if (admin)
                psi.Verb = "runas";
            if (workingDirectory == null)
                psi.WorkingDirectory = Helper.CurrentWorkingDirectoryOrDefault(progContext);
            else
                psi.WorkingDirectory = workingDirectory;
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
