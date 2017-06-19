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
using Chrome.BookmarkSearch;

namespace HakeQuick
{
    public sealed class Startup
    {
        public Startup()
        {

        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddPluginProvider();
        }
        public void ConfigureComponents(IAppBuilder app)
        {
            // bookmark search should not use parsed input arguments
            // so UseErrorBlocker must be put behind
            app.UseChromeBookmarkSearch();

            app.UseErrorBlocker(blockIfError: true);

            app.UsePlugins();
        }
    }
}
