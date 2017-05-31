using HakeQuick.Abstraction.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace HakeQuick.Implementation.Components.PluginLoader
{
    internal sealed class PluginMatchedMethodRecord
    {
        public object Instance { get; }
        public IEnumerable<MethodInfo> MatchedMethods { get; }

        public PluginMatchedMethodRecord(object instance, IEnumerable<MethodInfo> methods)
        {
            Instance = instance;
            MatchedMethods = methods;
        }
    }
    internal interface IPluginProvider: IDisposable
    {
        IEnumerable<PluginMatchedMethodRecord> MatchInputs(IQuickContext context);
    }
}
