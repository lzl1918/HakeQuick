using Hake.Extension.DependencyInjection.Abstraction;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace HakeQuick.Abstraction.Base
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder Use(this IAppBuilder app, Func<IQuickContext, Func<Task>, Task> component)
        {
            return app.Use(next =>
            {
                return context =>
                {
                    Func<Task> callnext = () => next(context);
                    return component(context, callnext);
                };
            });
        }
        public static IAppBuilder UseComponent<T>(this IAppBuilder app)
        {
            Type type = typeof(T);
            TypeInfo typeInfo = type.GetTypeInfo();
            if (typeInfo.IsAbstract == true)
                throw new InvalidOperationException($"cannot use an abstract class {type.FullName} as component");
            if (typeInfo.IsInterface == true)
                throw new InvalidOperationException($"cannot use an interface {type.FullName} as component");

            MethodInfo method = type.GetMethod("Invoke");
            if (method == null)
                throw new InvalidOperationException($"component does not contains any function named Invoke");
            if (method.ReturnType != typeof(Task))
                throw new InvalidOperationException($"function Invoke has invalid return type");

            return app.Use((context, next) =>
            {
                IServiceProvider services = app.Services;
                object instance = services.CreateInstance(type, next);
                object result = ObjectFactory.InvokeMethod(instance, method, services, context, next);
                if (result != null && result is Task)
                    return result as Task;
                return next();
            });
        }
    }
}
