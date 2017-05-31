using Hake.Extension.DependencyInjection.Abstraction;
using HakeQuick.Abstraction.Base;
using HakeQuick.Implementation.Services.Window;

namespace HakeQuick.Implementation.Base
{
    public static class QuickWindowExtensions
    {
        public static IHostBuilder UseWindow<T>(this IHostBuilder builder) where T : IQuickWindow
        {
            builder.ConfigureService(services =>
            {
                IServiceCollection pool = services.GetService(typeof(IServiceCollection)) as IServiceCollection;
                IQuickWindow window = services.CreateInstance<T>();
                pool.Add(ServiceDescriptor.Singleton<IQuickWindow>(window));
            });
            return builder;
        }
    }
}
