using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HakeQuick.Abstraction.Services
{
    public enum MessageType
    {
        Message,
        Warning,
        Error,
    }

    public interface ILogger
    {
        string Category { get; }

        Task LogAsync(MessageType type, string message);
    }

    public static class LoggerExtension
    {
        public static Task LogMessageAsync(this ILogger logger, string message)
        {
            return logger.LogAsync(MessageType.Message, message);
        }
        public static Task LogWarningAsync(this ILogger logger, string message)
        {
            return logger.LogAsync(MessageType.Warning, message);
        }
        public static Task LogErrorAsync(this ILogger logger, string message)
        {
            return logger.LogAsync(MessageType.Error, message);
        }
        public static Task LogExceptionAsync(this ILogger logger, Exception exception)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(exception.GetType().Name);
            builder.Append(':');
            builder.Append(' ');
            builder.Append(exception.Message);
            builder.AppendLine();
            builder.Append(exception.StackTrace);
            return logger.LogAsync(MessageType.Error, builder.ToString());
        }
    }
}
