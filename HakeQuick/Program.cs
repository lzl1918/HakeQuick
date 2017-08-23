using Hake.Extension.ValueRecord;
using Hake.Extension.ValueRecord.Mapper;
using HakeQuick.Abstraction.Base;
using HakeQuick.Implementation.Base;
using HakeQuick.Implementation.Configuration;
using HakeQuick.Implementation.Services.HotKey;
using HakeQuick.Implementation.Services.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HakeQuick
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddDefault()
                .TryAddJson("settings.json")
                .Build();
            QuickConfig options = configuration.Options;

            IHost host = new HostBuilder()
                .AddConfiguration(configuration)
                .UseEnvironment(plugin: "plugins", config: options.ConfigPath, log: options.LogPath)
                .AddFileLoggerFactory()
                .UseHotKey(key: options.Hotkey.Key, flags: options.Hotkey.KeyFlags)
                .UseWindow<DefaultWindow>()
                .UseConfiguration<Startup>()
                .Build();

            host.Run();
        }
    }
}
