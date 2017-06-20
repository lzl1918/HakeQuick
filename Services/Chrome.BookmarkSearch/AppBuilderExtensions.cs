using HakeQuick.Abstraction.Base;
using HakeQuick.Abstraction.Services;
using System.Linq;
using System.Text;

namespace Chrome.BookmarkSearch
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseChromeBookmarkSearch(this IAppBuilder app)
        {
            ICurrentEnvironment env = app.Services.GetService(typeof(ICurrentEnvironment)) as ICurrentEnvironment;
            ChromeBookmarkSearchComponent.Initialize(env);
            return app.UseComponent<ChromeBookmarkSearchComponent>();
        }
    }
}
