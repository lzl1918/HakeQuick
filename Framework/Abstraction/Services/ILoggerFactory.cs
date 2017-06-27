namespace HakeQuick.Abstraction.Services
{
    public interface ILoggerFactory
    {
        bool RemoveLogger(ILogger logger);
        bool RemoveLogger(string category);
        ILogger GetLogger(string category);
        ILogger CreateLogger(string category);
    }
}
