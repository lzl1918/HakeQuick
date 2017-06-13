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
    public static class HotKeyBuilderExtensions
    {
        public static IHostBuilder UseHotKey(this IHostBuilder builder, Key key = Key.Q, KeyFlags flags = KeyFlags.Control)
        {
            return builder.ConfigureService(services =>
            {
                IServiceCollection pool = services.GetService(typeof(IServiceCollection)) as IServiceCollection;
                IHotKeyBuilder hotkeyBuilder;
                if (services.TryGetService<IHotKeyBuilder>(out hotkeyBuilder) == false)
                {
                    hotkeyBuilder = new HotKeyBuilder();
                    hotkeyBuilder.SetBinding(key, flags);
                    pool.Add(ServiceDescriptor.Singleton<IHotKeyBuilder>(hotkeyBuilder));
                }
                else
                {
                    hotkeyBuilder.SetBinding(key, flags);
                }
            });
        }
    }
}
