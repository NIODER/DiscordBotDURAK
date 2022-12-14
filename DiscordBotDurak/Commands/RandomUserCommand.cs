using Discord;
using System;
using System.Collections.Generic;

namespace DiscordBotDurak.Commands
{
    public class RandomUserCommand : ICommand
    {
        private readonly List<ulong> users;
        private readonly long count;
        private readonly Exception exception;
        private readonly IGuild guild;

        public RandomUserCommand(List<ulong> users, long count, IGuild guild, Exception exception)
        {
            this.users = users;
            this.count = count;
            this.exception = exception;
            this.guild = guild;
        }

        public CommandResult GetResult()
        {
            CommandResult commandResult;
            if (exception is null)
            {
                var mentions = new List<string>();
                var random = new Random();
                for (long i = 0; i < count; i++)
                {
                    mentions.Add(guild.GetUserAsync(users[random.Next(users.Count)]).Result.Mention);
                }
                string response = string.Empty;
                foreach (var mention in mentions)
                {
                    response += mention;
                    response += " ";
                }
                commandResult = new CommandResult()
                    .WithEmbed(new EmbedBuilder()
                        .WithAuthor("DiscordBotDurak")
                        .WithColor(Color.Blue)
                        .AddField("Result:", response.Trim())
                        .WithFooter("Random user command")
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
