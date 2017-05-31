using HakeQuick.Abstraction.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HakeQuick.Implementation.Services.CurrentEnvironment
{
    internal sealed class CurrentEnvironment : ICurrentEnvironment
    {
        public DirectoryInfo MainDirectory { get; }

        public DirectoryInfo PluginDirectory { get; }

        public DirectoryInfo ConfigDirectory { get; }

        public CurrentEnvironment(string plugin, string config)
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
                Directory.CreateDirectory(configdir);
                ConfigDirectory = new DirectoryInfo(configdir);
            }
        }
    }
}
