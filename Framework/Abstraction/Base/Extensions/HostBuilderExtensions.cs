using Hake.Extension.DependencyInjection.Abstraction;
using System;

namespace HakeQuick.Abstraction.Base
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder AddConfiguration(this IHostBuilder builder, IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            return builder.ConfigureService(services =>
            {
                IServiceCollection pool = services.GetService<IServiceCollection>();
                pool.Add(ServiceDescriptor.Singleton<IConfiguration>(configuration));
            });
        }

        public static IHostBuilder ConfigureComponents<T>(this IHostBuilder builder)
        {
            return builder.ConfigureService(services =>
            {
                try
                {
                    object instance = services.CreateInstance<T>();
                    ObjectFactory.InvokeMethod(instance, "ConfigureComponents", services);
                }
                catch
                {

                }
            });
        }
        public static IHostBuilder ConfigureServices<T>(this IHostBuilder builder)
        {
            return builder.ConfigureService(services =>
            {
                try
                {
                    object instance = services.CreateInstance<T>();
                    ObjectFactory.InvokeMethod(instance, "ConfigureServices", services);
                }
                catch
                {

                }
            });
        }
        public static IHostBuilder UseConfiguration<T>(this IHostBuilder builder)
        {
            return builder.ConfigureService(services =>
            {
                try
                {
                    object instance = services.CreateInstance<T>();
                    ObjectFactory.InvokeMethod(instance, "ConfigureServices", services);
                    ObjectFactory.InvokeMethod(instance, "ConfigureComponents", services);
                }
                catch
                {

                }
            });
        }

    }
}
