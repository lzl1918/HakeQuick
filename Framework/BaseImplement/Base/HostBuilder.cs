using System;

using Hake.Extension.DependencyInjection.Abstraction;
using HakeQuick.Abstraction.Base;
using HakeQuick.Implementation.Services.Tray;
using HakeQuick.Implementation.Services.ProgramContext;
using HakeQuick.Abstraction.Services;
using HakeQuick.Implementation.Services.TerminationNotifier;

namespace HakeQuick.Implementation.Base
{
    public class HostBuilder : IHostBuilder
    {
        private IServiceProvider services;
        private IServiceCollection pool;
        private IAppBuilder app;

        public HostBuilder()
        {
            pool = Hake.Extension.DependencyInjection.Implementations.Implementation.CreateServiceCollection();
            services = Hake.Extension.DependencyInjection.Implementations.Implementation.CreateServiceProvider(pool);
            pool.Add(ServiceDescriptor.Singleton<IServiceCollection>(pool));
            pool.Add(ServiceDescriptor.Singleton<IServiceProvider>(services));

            ConfigureInternalServices();

            app = services.CreateInstance<AppBuilder>();
            pool.Add(ServiceDescriptor.Singleton<IAppBuilder>(app));
        }


        public IHost Build()
        {
            return services.CreateInstance<Host>();
        }

        public IHostBuilder ConfigureService(Action<IServiceProvider> configureServices)
        {
            if (configureServices == null)
                throw new ArgumentNullException(nameof(configureServices));
            configureServices.Invoke(services);
            return this;
        }

        private void ConfigureInternalServices()
        {
            ITerminationNotifier terminationNotifier = services.CreateInstance<TerminationNotifier>();
            pool.Add(ServiceDescriptor.Singleton<ITerminationNotifier>(terminationNotifier));

            ITray tray = services.CreateInstance<Tray>();
            pool.Add(ServiceDescriptor.Singleton<ITray>(tray));

            IProgramContextFactory programContextFactory = services.CreateInstance<ProgramContextFactory>();
            pool.Add(ServiceDescriptor.Singleton<IProgramContextFactory>(programContextFactory));
            pool.Add(ServiceDescriptor.Scoped<IProgramContext>(services =>
            {
                IProgramContextFactory factory = services.GetService<IProgramContextFactory>();
                return factory.RebuildContext();
            }));
        }
    }
}
