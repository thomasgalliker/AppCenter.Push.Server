namespace AppCenter.Push.Server.Logging
{
    public interface ILogger
    {
        void Log(LogLevel level, string message);
    }
}