using HakeQuick.Abstraction.Action;
using HakeQuick.Abstraction.Base;
using System;
using System.Diagnostics;
using System.Windows.Media.Imaging;

namespace RunnerPlugin
{
    public sealed class RunAction : ActionBase
    {
        public string RunCommand { get; }
        private string commandPath;
        public RunAction(string run, string path, string iconPath)
        {
            path = path ?? "";
            path = path.Trim();
            commandPath = path;
            IsExecutable = true;
            Title = "运行 - " + run;
            if (path.Length > 0)
                Subtitle = path + "/" + run;
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

        public void Invoke(ICommand command)
        {
            string procname = RunCommand;
            if (commandPath.Length > 0)
                procname = commandPath;
            ProcessStartInfo psi = new ProcessStartInfo(procname);
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
