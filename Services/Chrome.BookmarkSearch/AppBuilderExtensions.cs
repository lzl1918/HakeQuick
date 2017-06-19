using HakeQuick.Abstraction.Base;
using System.Linq;
using System.Text;

namespace Chrome.BookmarkSearch
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseChromeBookmarkSearch(this IAppBuilder app)
        {
            return app.UseComponent<ChromeBookmarkSearchComponent>();
        }
    }
}
