using HakeQuick.Abstraction.Base;

namespace HakeQuick.Implementation.Components.ErrorBlocker
{
    public static class ErrorBlockerExtensions
    {
        public static IAppBuilder UseErrorBlocker(this IAppBuilder app, bool blockIfError)
        {
            return app.UseComponent<ErrorBlocker>(blockIfError);
        }
    }
}
