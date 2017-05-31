using Hake.Extension.DependencyInjection.Abstraction;
using HakeQuick.Abstraction.Action;
using HakeQuick.Abstraction.Base;
using HakeQuick.Implementation.Components.PluginLoader;
using HakeQuick.Implementation.Components.ErrorBlocker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HakeQuick.Implementation.Services.Tray;

namespace HakeQuick
{
    public sealed class TestAction : ActionBase
    {
        public TestAction(string title, string subcontent)
        {
            Title = title;
            Subtitle = subcontent;
            IsExecutable = true;
        }

        public void Invoke(ITray tray)
        {
            tray.SendNotification(1000, "test", "content");
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
            app.UseErrorBlocker(blockIfError: true);
            app.UsePlugins();
        }
    }
}
