using System;

using Hake.Extension.DependencyInjection.Abstraction;
using HakeQuick.Abstraction.Services;

namespace HakeQuick.Implementation.Base
{
    internal static class LoggerMatchService
    {
        public static ILoggerFactory LoggerFactory { get; set; }
        private static Type ILoggerType { get; }
        static LoggerMatchService()
        {
            ObjectFactory.ParameterMatching += OnParameterMatching;
            ILoggerType = typeof(ILogger);
        }

        private static void OnParameterMatching(object sender, MatchingParameterEventArgs e)
        {
            if (e.ParameterType != ILoggerType)
                return;

            if (LoggerFactory == null)
                return;

            string name = e.ParameterName.ToLower();
            if (name.EndsWith("logger") && name.Length > 6)
                name = name.Substring(0, name.Length - 6);
            else if (name.EndsWith("log") && name.Length > 3)
                name = name.Substring(0, name.Length - 3);
            ILogger logger = LoggerFactory.GetLogger(name);
            if (logger != null)
                e.SetValue(logger);
        }
    }
}
