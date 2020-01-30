using System;
using AppCenter.Push.Server.Logging;

namespace AppCenter.Push.Server.Console.Internals
{
    internal class ConsoleLogger : ILogger
    {
        public void Log(LogLevel level, string message)
        {
            System.Console.WriteLine($"{DateTime.Now}|{level}|{message}");
        }
    }
}