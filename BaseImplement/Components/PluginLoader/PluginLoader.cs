using HakeQuick.Abstraction.Base;
using HakeQuick.Abstraction.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HakeQuick.Implementation.Components.PluginLoader
{
    internal sealed class PluginComponent
    {

        private IPluginProvider plugins;
        public PluginComponent(IPluginProvider plugins)
        {
            this.plugins = plugins;
        }

        public Task Invoke(IQuickContext context, Func<Task> next)
        {
            return next();
        }
    }
}
