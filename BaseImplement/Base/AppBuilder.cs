using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hake.Extension.Pipeline.Implementations;
using HakeQuick.Abstraction.Base;

namespace HakeQuick.Implementation.Base
{
    public sealed class AppBuilder : PipelineBase<AppDelegate, IAppBuilder, IQuickContext>, IAppBuilder
    {
        private static Task COMPLETED_TASK { get; } = Task.Run(()=> { });
        private static AppDelegate BASE_COMPONENT { get; } = context => COMPLETED_TASK;

        public IServiceProvider Services { get; }

        public AppBuilder(IServiceProvider services) : base(BASE_COMPONENT)
        {
            Services = services;
        }
    }
}
