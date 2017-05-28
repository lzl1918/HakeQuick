using Hake.Extension.DependencyInjection.Abstraction;
using HakeQuick.Abstraction.Base;
using HakeQuick.Implementation.Services.HotKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HakeQuick.Implementation.Base
{
    public static class HotKeyExtensions
    {
        public static IHostBuilder UseHotKey(this IHostBuilder builder, Key key = Key.Q, KeyFlags flags = KeyFlags.Control)
        {
            builder.ConfigureService(services =>
            {
                IServiceCollection pool = services.GetService(typeof(IServiceCollection)) as IServiceCollection;
                IHotKey hotkey = new HotKey();
                hotkey.BindKey(key, flags);
                pool.Add(ServiceDescriptor.Singleton<IHotKey>(hotkey));
            });
            return builder;
        }
    }
}
