using Hake.Extension.DependencyInjection.Abstraction;

namespace HakeQuick.Abstraction.Base
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder ConfigureComponents<T>(this IHostBuilder builder)
        {
            builder.ConfigureService(services =>
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
            return builder;
        }
        public static IHostBuilder ConfigureServices<T>(this IHostBuilder builder)
        {
            builder.ConfigureService(services =>
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
            return builder;
        }
        public static IHostBuilder UseConfiguration<T>(this IHostBuilder builder)
        {
            builder.ConfigureService(services =>
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
            return builder;
        }

    }
}
