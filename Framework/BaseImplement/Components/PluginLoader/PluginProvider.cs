using System;
using System.Collections.Generic;
using HakeQuick.Abstraction.Base;
using HakeQuick.Abstraction.Services;
using System.IO;
using System.Reflection;
using HakeQuick.Abstraction.Plugin;
using Hake.Extension.DependencyInjection.Abstraction;
using HakeQuick.Abstraction.Action;
using System.Linq;

namespace HakeQuick.Implementation.Components.PluginLoader
{
    internal sealed class PluginProvider : IPluginProvider
    {
        private sealed class ExplictMethodRecord
        {
            public object Instance { get; }
            public IReadOnlyList<MethodInfo> Methods { get; }
            public ExplictMethodRecord(object instance, IReadOnlyList<MethodInfo> methods)
            {
                Instance = instance;
                Methods = methods;
            }
        }
        private sealed class PluginMethodRecord
        {
            public object Instance { get; }
            public IReadOnlyDictionary<string, List<MethodInfo>> ActionEntry { get; }
            public PluginMethodRecord(object instance, IReadOnlyDictionary<string, List<MethodInfo>> actionEntry)
            {
                Instance = instance;
                ActionEntry = actionEntry;
            }
        }
        private sealed class IgnoreIdentityRecords
        {
            public object Instance { get; }
            public IReadOnlyList<MethodInfo> Methods { get; }
            public IgnoreIdentityRecords(object instance, IReadOnlyList<MethodInfo> methods)
            {
                Instance = instance;
                Methods = methods;
            }
        }

        private Dictionary<string, List<PluginMethodRecord>> pluginEntry;
        private Dictionary<string, List<IgnoreIdentityRecords>> ignoreIdentityEntry;
        private List<ExplictMethodRecord> explictEntry;
        private List<object> instances;

        public PluginProvider(ICurrentEnvironment env, IServiceProvider services)
        {
            DirectoryInfo pluginDirectory = env.PluginDirectory;
            LoadPlugins(pluginDirectory, services);
        }

        public IEnumerable<PluginMatchedMethodRecord> MatchInputs(IQuickContext context)
        {
            ICommand command = context.Command;
            if (command.ContainsError) return new PluginMatchedMethodRecord[0];

            List<PluginMatchedMethodRecord> results = new List<PluginMatchedMethodRecord>();
            Dictionary<object, List<MethodInfo>> temp_results = new Dictionary<object, List<MethodInfo>>();
            string action = command.Action;
            string identity = command.Identity;
            object instance;
            SortedSet<int> methodMap = new SortedSet<int>();
            if (identity.Length <= 0)
            {
                // empty
                foreach (var pair in ignoreIdentityEntry)
                {
                    foreach (IgnoreIdentityRecords rec in pair.Value)
                    {
                        List<MethodInfo> methods;
                        if (temp_results.TryGetValue(rec.Instance, out methods) == false)
                        {
                            methods = new List<MethodInfo>();
                            temp_results.Add(rec.Instance, methods);
                        }
                        foreach (MethodInfo method in rec.Methods)
                        {
                            if (methodMap.Contains(method.GetHashCode()))
                                continue;
                            methods.Add(method);
                            methodMap.Add(method.GetHashCode());
                        }
                    }
                }
            }
            else
            {
                if (action.Length > 0)
                {
                    List<PluginMethodRecord> records;
                    if (pluginEntry.TryGetValue(command.Identity, out records))
                    {
                        foreach (PluginMethodRecord record in records)
                        {
                            foreach (var pair in record.ActionEntry)
                            {
                                if (pair.Key.Length >= action.Length && pair.Key.StartsWith(action))
                                {
                                    List<MethodInfo> methods;
                                    if (temp_results.TryGetValue(record.Instance, out methods) == false)
                                    {
                                        methods = new List<MethodInfo>();
                                        temp_results.Add(record.Instance, methods);
                                    }
                                    foreach (MethodInfo method in pair.Value)
                                    {
                                        if (methodMap.Contains(method.GetHashCode()))
                                            continue;
                                        methods.Add(method);
                                        methodMap.Add(method.GetHashCode());
                                    }
                                }
                            }
                        }
                    }
                    List<IgnoreIdentityRecords> ignoreIdentityRecords;
                    if (ignoreIdentityEntry.TryGetValue(command.Identity, out ignoreIdentityRecords))
                    {
                        foreach (IgnoreIdentityRecords record in ignoreIdentityRecords)
                        {
                            List<MethodInfo> methods;
                            if (temp_results.TryGetValue(record.Instance, out methods) == false)
                            {
                                methods = new List<MethodInfo>();
                                temp_results.Add(record.Instance, methods);
                            }
                            foreach (MethodInfo method in record.Methods)
                            {
                                if (methodMap.Contains(method.GetHashCode()))
                                    continue;
                                methods.Add(method);
                                methodMap.Add(method.GetHashCode());
                            }
                        }
                    }
                }
                else
                {
                    // 只有identity
                    foreach (var pair in pluginEntry)
                    {
                        if (pair.Key.Length >= identity.Length && pair.Key.StartsWith(identity))
                        {
                            foreach (PluginMethodRecord rec in pair.Value)
                            {
                                List<MethodInfo> methods;
                                if (temp_results.TryGetValue(rec.Instance, out methods) == false)
                                {
                                    methods = new List<MethodInfo>();
                                    temp_results.Add(rec.Instance, methods);
                                }
                                foreach (List<MethodInfo> methodswithaction in rec.ActionEntry.Values)
                                {
                                    foreach (MethodInfo method in methodswithaction)
                                    {
                                        if (methodMap.Contains(method.GetHashCode()))
                                            continue;
                                        methods.Add(method);
                                        methodMap.Add(method.GetHashCode());
                                    }
                                }
                            }
                        }
                    }
                    foreach (var pair in ignoreIdentityEntry)
                    {
                        if (pair.Key.Length >= identity.Length && pair.Key.StartsWith(identity))
                        {
                            foreach (IgnoreIdentityRecords rec in pair.Value)
                            {
                                List<MethodInfo> methods;
                                if (temp_results.TryGetValue(rec.Instance, out methods) == false)
                                {
                                    methods = new List<MethodInfo>();
                                    temp_results.Add(rec.Instance, methods);
                                }
                                foreach (MethodInfo method in rec.Methods)
                                {
                                    if (methodMap.Contains(method.GetHashCode()))
                                        continue;
                                    methods.Add(method);
                                    methodMap.Add(method.GetHashCode());
                                }
                            }
                        }
                    }
                }
            }

            // explicts
            foreach (ExplictMethodRecord explict in explictEntry)
            {
                instance = explict.Instance;
                IReadOnlyList<MethodInfo> methods = explict.Methods;
                List<MethodInfo> result_methods;
                if (temp_results.TryGetValue(instance, out result_methods) == false)
                {
                    result_methods = new List<MethodInfo>();
                    temp_results.Add(instance, result_methods);
                }
                foreach (MethodInfo method in methods)
                {
                    if (methodMap.Contains(method.GetHashCode()))
                        continue;
                    result_methods.Add(method);
                    methodMap.Add(method.GetHashCode());
                }
            }

            foreach (var pair in temp_results)
            {
                results.Add(new PluginMatchedMethodRecord(pair.Key, pair.Value));
            }
            return results;
        }

        private void LoadPlugins(DirectoryInfo pluginDirectory, IServiceProvider services)
        {
            pluginEntry = new Dictionary<string, List<PluginMethodRecord>>();
            ignoreIdentityEntry = new Dictionary<string, List<IgnoreIdentityRecords>>();
            explictEntry = new List<ExplictMethodRecord>();
            instances = new List<object>();

            FileInfo[] files = pluginDirectory.GetFiles("*.dll");
            Type pluginBaseType = typeof(QuickPlugin);
            Type actionUpdateResultType = typeof(ActionUpdateResult);
            Type asyncActionUpdateType = typeof(AsyncActionUpdate);
            Type enumActionUpdateResultType = typeof(IEnumerable<ActionUpdateResult>);
            Type enumAsyncActionUpdateType = typeof(IEnumerable<AsyncActionUpdate>);
            foreach (FileInfo file in files)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFile(file.FullName);
                    Type[] types = assembly.GetTypes();
                    foreach (Type type in types)
                    {
                        TypeInfo typeInfo = type.GetTypeInfo();

                        if (typeInfo.IsArray == true) continue;
                        if (typeInfo.IsEnum == true) continue;
                        if (typeInfo.IsAbstract == true) continue;
                        if (typeInfo.IsInterface == true) continue;
                        if (typeInfo.IsClass == false) continue;
                        if (!pluginBaseType.IsAssignableFrom(type)) continue;

                        List<MethodInfo> explictMethods = new List<MethodInfo>();
                        Dictionary<string, List<MethodInfo>> ignoreIdentityMethods = new Dictionary<string, List<MethodInfo>>();
                        Dictionary<string, List<MethodInfo>> actionMethods = new Dictionary<string, List<MethodInfo>>();
                        object instance;
                        SortedSet<string> nameMap = new SortedSet<string>();
                        foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                        {
                            if (actionUpdateResultType.IsAssignableFrom(method.ReturnType) == false &&
                                asyncActionUpdateType.IsAssignableFrom(method.ReturnType) == false &&
                                enumActionUpdateResultType.IsAssignableFrom(method.ReturnType) == false &&
                                enumAsyncActionUpdateType.IsAssignableFrom(method.ReturnType) == false)
                                continue;

                            // ExplicitCallAttribute
                            {
                                ExplicitCallAttribute explicitAttributes = method.GetCustomAttribute<ExplicitCallAttribute>();
                                if (explicitAttributes != null)
                                {
                                    explictMethods.Add(method);
                                    continue;
                                }
                            }

                            // IgnoreIdentityAttribute
                            {
                                nameMap.Clear();
                                IEnumerable<IgnoreIdentityAttribute> ignoreIdentityAttributes = method.GetCustomAttributes<IgnoreIdentityAttribute>();
                                string identity;
                                foreach (IgnoreIdentityAttribute ignoreIdentityAttribute in ignoreIdentityAttributes)
                                {
                                    identity = ignoreIdentityAttribute.Name.Trim().ToLower();
                                    if (identity.Length <= 0)
                                        continue;
                                    if (nameMap.Contains(identity))
                                        continue;
                                    if (ignoreIdentityMethods.TryGetValue(identity, out List<MethodInfo> methods) == true)
                                    {
                                        methods.Add(method);
                                    }
                                    else
                                    {
                                        ignoreIdentityMethods[identity] = new List<MethodInfo>() { method };
                                    }
                                    nameMap.Add(identity);
                                }
                            }

                            // ActionAttribute
                            {
                                nameMap.Clear();
                                IEnumerable<ActionAttribute> actionAttributes = method.GetCustomAttributes<ActionAttribute>();
                                string action;
                                foreach (ActionAttribute actionAttribute in actionAttributes)
                                {
                                    action = actionAttribute.Name.Trim().ToLower();
                                    if (action.Length <= 0)
                                        continue;
                                    if (nameMap.Contains(action))
                                        continue;
                                    if (actionMethods.TryGetValue(action, out List<MethodInfo> methods) == true)
                                    {
                                        methods.Add(method);
                                    }
                                    else
                                    {
                                        actionMethods[action] = new List<MethodInfo>() { method };
                                    }
                                    nameMap.Add(action);
                                }
                            }

                        }
                        if (explictMethods.Count + ignoreIdentityMethods.Count + actionMethods.Count <= 0)
                            continue;

                        instance = services.CreateInstance(type);
                        instances.Add(instance);
                        if (explictMethods.Count > 0)
                        {
                            ExplictMethodRecord record = new ExplictMethodRecord(instance, explictMethods);
                            explictEntry.Add(record);
                        }
                        foreach (var pair in ignoreIdentityMethods)
                        {
                            IgnoreIdentityRecords record = new IgnoreIdentityRecords(instance, pair.Value);
                            if (ignoreIdentityEntry.TryGetValue(pair.Key, out List<IgnoreIdentityRecords> methods))
                                methods.Add(record);
                            else
                                ignoreIdentityEntry.Add(pair.Key, new List<IgnoreIdentityRecords>() { record });
                        }
                        if (actionMethods.Count > 0)
                        {
                            IEnumerable<IdentityAttribute> identityAttributes = type.GetCustomAttributes<IdentityAttribute>();
                            if (identityAttributes.Count() > 0)
                            {
                                nameMap.Clear();
                                string identity;
                                PluginMethodRecord record = new PluginMethodRecord(instance, actionMethods);
                                foreach (IdentityAttribute identityAttribute in identityAttributes)
                                {
                                    identity = identityAttribute.Name.Trim().ToLower();
                                    if (nameMap.Contains(identity))
                                        continue;
                                    if (pluginEntry.TryGetValue(identity, out List<PluginMethodRecord> methods))
                                        methods.Add(record);
                                    else
                                        pluginEntry.Add(identity, new List<PluginMethodRecord>() { record });
                                    nameMap.Add(identity);
                                }
                            }
                        }
                    }
                }
                catch { }
            }
        }

        private bool disposed = false;
        ~PluginProvider()
        {
            if (!disposed)
                Dispose();
        }
        public void Dispose()
        {
            if (disposed) return;

            foreach (object instance in instances)
            {
                if (instance is IDisposable dis)
                    dis.Dispose();
            }
            disposed = true;
        }
    }
}
