using Hake.Extension.DependencyInjection.Abstraction;
using HakeQuick.Abstraction.Action;
using HakeQuick.Abstraction.Base;
using HakeQuick.Implementation.Components.PluginLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HakeQuick
{
    public sealed class TestAction : ActionBase
    {
        public TestAction(string title, string subcontent)
        {
            Title = title;
            Subtitle = subcontent;
        }
    }

    public sealed class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddPluginProvider();
        }
        public void ConfigureComponents(IAppBuilder app)
        {
            app.UsePlugins();
            app.Use((context, next) =>
            {
                context.AddAction(new TestAction("ab", "abc"));
                return next();
            });
        }
    }
}
