using HakeQuick.Abstraction.Base;
using HakeQuick.Implementation.Base;
using HakeQuick.Implementation.Services.HotKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HakeQuick
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            IHost host = new HostBuilder()
                .UseEnvironment(plugin: "/plugins", config: "/config")
                .UseHotKey(key: Key.Q, flags: KeyFlags.Control)
                .UseWindow<DefaultWindow>()
                .UseConfiguration<Startup>()
                .Build();

            host.Run();
        }
    }
}
