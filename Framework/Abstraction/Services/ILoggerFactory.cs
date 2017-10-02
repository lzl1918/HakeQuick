namespace HakeQuick.Abstraction.Services
{
    public interface ILoggerFactory
    {
        bool RemoveLogger(ILogger logger);
        bool RemoveLogger(string category);
        ILogger CreateLogger(string category);
        ILogger GetLogger(string category);
    }

    public static class LoggerFactoryExtensions
    {
        public static ILogger GetLogger<T>(this ILoggerFactory loggerFactory) => loggerFactory.GetLogger(typeof(T).Name);
        public static ILogger CreateLogger<T>(this ILoggerFactory loggerFactory) => loggerFactory.CreateLogger(typeof(T).Name);
        public static bool RemoveLogger<T>(this ILoggerFactory loggerFactory) => loggerFactory.RemoveLogger(typeof(T).Name);
    }
}
