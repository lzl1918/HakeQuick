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
            IConfiguration configuration = new ConfigurationBuilder()
                .AddDefault()
                .TryAddJson("settings.json")
                .Build();


            string config_path;
            Key hotkey;
            KeyFlags hotkeyFlags;
            try
            {
                ReadConfig(configuration, out config_path, out hotkey, out hotkeyFlags);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            IHost host = new HostBuilder()
                .AddConfiguration(configuration)
                .UseEnvironment(plugin: "plugins", config: config_path)
                .UseHotKey(key: hotkey, flags: hotkeyFlags)
                .UseWindow<DefaultWindow>()
                .UseConfiguration<Startup>()
                .Build();

            host.Run();
        }

        private static void ReadConfig(IConfiguration config, out string config_path, out Key hotkey, out KeyFlags hotkeyFlags)
        {
            config_path = config.Root.ReadAs<string>("config");
            hotkey = (Key)Enum.Parse(typeof(Key), config.Root.ReadAs<string>("hotkey.key"));
            string[] keyflags = config.Root.ReadAs<string>("hotkey.flags").Split('+');
            hotkeyFlags = KeyFlags.None;
            if (keyflags.Length > 0)
            {
                foreach (string keyflag in keyflags)
                {
                    hotkeyFlags |= (KeyFlags)Enum.Parse(typeof(KeyFlags), keyflag);
                }
            }
            if (hotkeyFlags == KeyFlags.None)
                hotkeyFlags = KeyFlags.Control;
        }
    }
}
