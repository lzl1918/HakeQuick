using HakeQuick.Abstraction.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace HakeQuick.Implementation.Services.CurrentEnvironment
{
    internal sealed class CurrentEnvironment : ICurrentEnvironment
    {
        public DirectoryInfo MainDirectory { get; }

        public DirectoryInfo PluginDirectory { get; }

        public DirectoryInfo ConfigDirectory { get; }
        public DirectoryInfo LogDirectory { get; }

        public CurrentEnvironment(string plugin, string config, string log)
        {
            string current = Directory.GetCurrentDirectory();
            MainDirectory = new DirectoryInfo(current);
            string plugindir = Path.Combine(current, plugin);
            PluginDirectory = new DirectoryInfo(plugindir);
            if (!PluginDirectory.Exists)
            {
                Directory.CreateDirectory(plugindir);
                PluginDirectory = new DirectoryInfo(plugindir);
            }
            string configdir = Path.Combine(current, config);
            ConfigDirectory = new DirectoryInfo(configdir);
            if (!ConfigDirectory.Exists)
            {
                try
                {
                    Directory.CreateDirectory(configdir);
                    ConfigDirectory = new DirectoryInfo(configdir);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            string logdir = Path.Combine(current, log);
            LogDirectory = new DirectoryInfo(logdir);
            if (!LogDirectory.Exists)
            {
                try
                {
                    Directory.CreateDirectory(logdir);
                    LogDirectory = new DirectoryInfo(logdir);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
