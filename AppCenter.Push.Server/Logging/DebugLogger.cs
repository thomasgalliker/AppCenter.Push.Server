using System;
using System.Diagnostics;

namespace AppCenter.Push.Server.Logging
{
    internal class DebugLogger : ILogger
    {
        public void Log(LogLevel level, string message)
        {
            Debug.WriteLine($"{DateTime.UtcNow}|Paging.NET|{level}|{message}[EOL]");
        }
    }
}