using Discord;
using DiscordBotDurak.Data;
using System;
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
            if (logMessage.Severity <= minimalLogSeverity)
            {
                // log in db
                using var db = new Database();
                db.BeginTransaction();
                await db.LogInDatabase(logMessage);

                // log in console
                lock (consoleSyncObj)
                {
                    Console.WriteLine(logMessage.ToString());
                }
            }
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
