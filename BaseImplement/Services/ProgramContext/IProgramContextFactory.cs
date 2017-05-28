using System;
using HakeQuick.Abstraction.Services;
using Hake.Extension.DependencyInjection.Abstraction;

namespace HakeQuick.Implementation.Services.ProgramContext
{
    public interface IProgramContextFactory
    {
        IProgramContext Context { get; }
        IProgramContext RebuildContext();
    }

    internal sealed class ProgramContextFactory : IProgramContextFactory
    {
        private IServiceProvider services;
        private IProgramContext context;
        public IProgramContext Context { get { return context; } }

        public ProgramContextFactory(IServiceProvider services)
        {
            this.services = services;
        }


        public IProgramContext RebuildContext()
        {
            context = services.CreateInstance<ProgramContext>();
            return context;
        }
    }
}
