using Discord;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBotDurak
{
    public class Logger
    {
        private static readonly object consoleSyncObj = new();
        private readonly LogSeverity minimalLogSeverity;
        private static Logger instance;

        private Logger(LogSeverity minimalLogSeverity)
        {
            this.minimalLogSeverity = minimalLogSeverity;
        }

        public static Logger Instance(LogSeverity logSeverity = LogSeverity.Debug)
        {
            if (instance is null)
                instance = new Logger(logSeverity);
            return instance;
        }

        public async Task LogAsync(LogMessage logMessage)
        {
            await Task.Run(() =>
            {
                if (logMessage.Severity <= minimalLogSeverity)
                {
                    lock (consoleSyncObj)
                    {
                        Console.WriteLine(logMessage.ToString());
                    }
                }
            });
            // log in database
        }

        public static void Log(
            LogSeverity logSeverity,
            string source,
            string message,
            Exception exception = null)
        {
            _ = Instance().LogAsync(new LogMessage(logSeverity, source, message, exception));
        }
    }
}
