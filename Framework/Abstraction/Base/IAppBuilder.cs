using Hake.Extension.Pipeline.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HakeQuick.Abstraction.Base
{
    public delegate Task AppDelegate(IQuickContext context);

    public interface IAppBuilder : IPipeline<AppDelegate, IAppBuilder, IQuickContext>
    {
        IServiceProvider Services { get; }
    }
}
