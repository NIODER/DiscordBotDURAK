using Discord;
using System;

namespace DiscordBotDurak.Commands
{
    public class RandomNumberCommand : ICommand
    {
        private readonly long min;
        private readonly long max;
        private readonly long count;
        private readonly Exception exception;

        public RandomNumberCommand(long min, long max, long count, Exception exception)
        {
            this.max = max;
            this.min = min;
            this.count = count;
            this.exception = exception;
        }

        public CommandResult GetResult()
        {
            CommandResult commandResult;
            if (exception is null)
            {
                var response = string.Empty;
                var random = new Random();
                for (int i = 0; i < count; i++)
                {
                    response += random.Next(min, max);
                    response += " ";
                }
                response = response.TrimEnd();
                commandResult = new CommandResult()
                    .WithEmbed(new EmbedBuilder()
                        .WithColor(Color.Blue)
                        .WithAuthor("DiscordBotDurak")
                        .AddField("Result:", response)
                        .WithFooter("Random number command")
                        .WithCurrentTimestamp());
                _ = Logger.Instance().LogAsync(new LogMessage(LogSeverity.Info, GetType().Name, "Success."));
            }
            else
            {
                commandResult = new CommandResult().WithException(exception);
                _ = Logger.Instance().LogAsync(new LogMessage(LogSeverity.Info, GetType().Name, "Error occured.", exception));
            }
            return commandResult;
        }
    }
}
