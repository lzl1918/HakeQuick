using HakeQuick.Abstraction.Action;
using HakeQuick.Abstraction.Base;
using HakeQuick.Abstraction.Services;
using System;
using System.Diagnostics;
using System.Windows.Media.Imaging;

namespace RunnerPlugin
{
    internal sealed class RunCommandAction : ActionBase
    {
        private bool defaultAsAdmin;
        private string workingDirectory;
        public string RunCommand { get; }
        private string commandPath;
        public RunCommandAction(string run, string path, string iconPath, bool admin, string workingDirectory)
        {
            defaultAsAdmin = admin;
            this.workingDirectory = workingDirectory;
            path = path ?? "";
            path = path.Trim();
            commandPath = path;
            IsExecutable = true;
            Title = "运行 - " + run;
            if (path.Length > 0)
                Subtitle = path;
            else
                Subtitle = run;
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

        public void Invoke(IProgramContext progContext, ICommand command, bool admin = false, bool a = false)
        {
            admin = admin || a || defaultAsAdmin;
            string procname = RunCommand;
            if (commandPath.Length > 0)
                procname = commandPath;
            ProcessStartInfo psi = new ProcessStartInfo(procname);
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
            catch
            {
            }
        }
    }
}
