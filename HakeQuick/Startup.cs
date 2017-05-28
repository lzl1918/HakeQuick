using HakeQuick.Abstraction.Action;
using HakeQuick.Abstraction.Base;
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
        public void ConfigureServices()
        {

        }
        public void ConfigureComponents(IAppBuilder app)
        {
            app.Use((context, next) =>
            {
                context.AddAction(new TestAction("ab", "abc"));
                return next();
            });
        }
    }
}
