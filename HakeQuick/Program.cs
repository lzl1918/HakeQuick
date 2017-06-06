using HakeQuick.Abstraction.Base;
using HakeQuick.Implementation.Base;
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
            try
            {
                Stream file_stream = File.OpenRead("settings.json");
                TextReader file_reader = new StreamReader(file_stream);
                JsonTextReader reader = new JsonTextReader(file_reader);
                JsonSerializer serializer = new JsonSerializer();
                JObject settings = serializer.Deserialize(reader) as JObject;
                reader.Close();
                file_stream.Dispose();
                config_path = settings["config_path"].Value<string>();
            }
            catch (FileNotFoundException)
            {
                config_path = "config";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }


            IHost host = new HostBuilder()
                .UseEnvironment(plugin: "plugins", config: config_path)
                .UseHotKey(key: Key.Q, flags: KeyFlags.Control)
                .UseWindow<DefaultWindow>()
                .UseConfiguration<Startup>()
                .Build();

            host.Run();
        }
    }
}
