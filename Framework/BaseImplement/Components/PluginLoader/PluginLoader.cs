using Hake.Extension.DependencyInjection.Abstraction;
using HakeQuick.Abstraction.Action;
using HakeQuick.Abstraction.Base;
using HakeQuick.Abstraction.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HakeQuick.Implementation.Components.PluginLoader
{
    internal sealed class PluginLoader
    {
        private IPluginProvider plugins;
        public PluginLoader(IPluginProvider plugins)
        {
            this.plugins = plugins;
        }

        public Task Invoke(IQuickContext context, Func<Task> next, IServiceProvider services)
        {
            object[] args = new object[context.Command.UnnamedArguments.Count + 2];
            args[0] = context;
            args[1] = context.Command;
            if (context.Command.UnnamedArguments.Count > 0)
                context.Command.UnnamedArguments.CopyTo(args, 2);
            IEnumerable<PluginMatchedMethodRecord> records = plugins.MatchInputs(context);
            foreach (PluginMatchedMethodRecord record in records)
            {
                foreach (MethodInfo method in record.MatchedMethods)
                {
                    try
                    {
                        object returnvalue = ObjectFactory.InvokeMethod(record.Instance, method, services, context.Command.NamedArguments, args);
                        if (returnvalue == null)
                            continue;

                        if (returnvalue is ActionUpdateResult action)
                        {
                            context.AddAction(action.Action, action.Priority);
                        }
                        else if (returnvalue is AsyncActionUpdate asyncAction)
                        {
                            context.AddAsyncAction(asyncAction);
                        }
                        else if (returnvalue is IEnumerable<ActionUpdateResult> actions)
                        {
                            foreach (ActionUpdateResult act in actions)
                                context.AddAction(act.Action, act.Priority);
                        }
                        else if (returnvalue is IEnumerable<AsyncActionUpdate> asyncActions)
                        {
                            context.AddAsyncActions(asyncActions);
                        }
                    }
                    catch { }
                }
            }
            return next();
        }
    }
}
