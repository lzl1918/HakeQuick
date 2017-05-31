using System.Collections.Generic;
using HakeQuick.Abstraction.Action;

namespace HakeQuick.Abstraction.Base
{
    public static class QuickContextExtension
    {
        public static void AddAsyncActions(this IQuickContext context, IEnumerable<AsyncActionUpdate> asyncActions)
        {
            foreach (AsyncActionUpdate asyncAction in asyncActions)
                context.AddAsyncAction(asyncAction);
        }
    }
}
