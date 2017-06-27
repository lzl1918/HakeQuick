using Hake.Extension.DependencyInjection.Abstraction;
using HakeQuick.Abstraction.Services;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HakeQuick.Implementation.Services.Logger
{
    internal sealed class Logger : ILogger
    {
        public string Category { get; }

        public Logger(string category, ICurrentEnvironment env)
        {
            FileLogger.Initialize(env);
            Category = category;
        }

        public Task LogAsync(MessageType type, string message)
        {
            return FileLogger.LogAsync($"[{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")}][{Category}][{type}] {message}");
        }
    }
}
