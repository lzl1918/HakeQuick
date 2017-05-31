using Hake.Extension.DependencyInjection.Abstraction;
using HakeQuick.Abstraction.Base;
using System;

namespace HakeQuick.Implementation.Components.PluginLoader
{
    public static class PluginLoaderExtensions
    {
        public static IServiceCollection AddPluginProvider(this IServiceCollection pool)
        {
            IServiceProvider services = pool.GetDescriptor<IServiceProvider>().GetInstance() as IServiceProvider;
            IPluginProvider provider = services.CreateInstance<PluginProvider>();
            pool.Add(ServiceDescriptor.Singleton<IPluginProvider>(provider));
            return pool;
        }
        public static IAppBuilder UsePlugins(this IAppBuilder builder)
        {
            return builder.UseComponent<PluginLoader>();
        }
    }
}
