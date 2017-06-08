using Hake.Extension.ValueRecord;
using HakeQuick.Abstraction.Base;
using HakeQuick.Implementation.Base;
using HakeQuick.Implementation.Configuration;
using HakeQuick.Implementation.Services.HotKey;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

            string config_path = "config";
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJson("settings.json")
                .Build();

            try
            {
                config_path = configuration.Root.ReadAs<string>("config");
            }
            catch (FileNotFoundException)
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            IHost host = new HostBuilder()
                .AddConfiguration(configuration)
                .UseEnvironment(plugin: "plugins", config: config_path)
                .UseHotKey(key: Key.Q, flags: KeyFlags.Control)
                .UseWindow<DefaultWindow>()
                .UseConfiguration<Startup>()
                .Build();

            host.Run();
        }
    }
}
