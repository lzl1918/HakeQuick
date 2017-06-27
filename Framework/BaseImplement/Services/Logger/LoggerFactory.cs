using Hake.Extension.DependencyInjection.Abstraction;
using HakeQuick.Abstraction.Base;
using HakeQuick.Abstraction.Services;
using HakeQuick.Implementation.Base;
using System;
using System.Collections.Generic;

namespace HakeQuick.Implementation.Services.Logger
{
    public static class LoggerFactoryExtensions
    {
        public static IHostBuilder AddFileLoggerFactory(this IHostBuilder builder)
        {
            return builder.ConfigureService(services =>
            {
                IServiceCollection pool = services.GetService<IServiceCollection>();
                ILoggerFactory loggerFactory = services.CreateInstance<LoggerFactory>();
                pool.Add(ServiceDescriptor.Singleton<ILoggerFactory>(loggerFactory));
                LoggerMatchService.LoggerFactory = loggerFactory;
            });
        }
    }
    internal sealed class LoggerFactory : ILoggerFactory, IDisposable
    {
        private IServiceProvider services;
        private Dictionary<string, ILogger> loggers = new Dictionary<string, ILogger>();
        private ILogger internalLogger;
        public LoggerFactory(IServiceProvider services)
        {
            this.services = services;

            internalLogger = CreateLogger("LoggerFactory");
        }

        public ILogger CreateLogger(string category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));
            string rawCate = category;
            category = category.ToLower();
            if (loggers.TryGetValue(category, out ILogger logger))
                return logger;
            else
            {
                IReadOnlyDictionary<string, object> args = new Dictionary<string, object>() { ["category"] = rawCate };
                ILogger createdLogger = services.CreateInstance<Logger>(args);
                loggers.Add(category, createdLogger);
                if (internalLogger != null)
                    internalLogger.LogMessageAsync($"New logger {category} created");
                return createdLogger;
            }
        }

        public ILogger GetLogger(string category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));
            if (loggers.TryGetValue(category, out ILogger logger))
                return logger;
            else
                return null;
        }

        public bool RemoveLogger(ILogger logger)
        {
            string key = null;
            foreach (var pair in loggers)
            {
                if (pair.Value == logger)
                {
                    key = pair.Key;
                    break;
                }
            }
            if (key == null)
                return false;
            else
                return loggers.Remove(key);
        }

        public bool RemoveLogger(string category)
        {
            return loggers.Remove(category);
        }

        #region IDisposable Support
        private bool disposedValue = false;

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                internalLogger.LogMessageAsync($"Disposing LoggerFactory");
                FileLogger.Dispose();
                disposedValue = true;
            }
        }
        ~LoggerFactory()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
