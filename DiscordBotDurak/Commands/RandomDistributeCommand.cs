using Discord;
using System;
using System.Collections.Generic;

namespace DiscordBotDurak.Commands
{
    public class RandomDistributeCommand : ICommand
    {
        private readonly List<IGuildUser> users;
        private readonly long teamsCount;
        private long teamsSize;
        private readonly Exception exception;

        public RandomDistributeCommand(List<IGuildUser> users, long teamsCount, long teamsSize, Exception exception)
        {
            this.users = users;
            this.teamsCount = teamsCount;
            this.teamsSize = teamsSize;
            this.exception = exception;
        }

        public CommandResult GetResult()
        {
            CommandResult commandResult;
            if (exception is null)
            {
                var random = new Random();
                for (int i = users.Count - 1; i >= 0; i--)
                {
                    int rindex = random.Next(i);
                    var temp = users[rindex];
                    users[rindex] = users[i];
                    users[i] = temp;
                }
                var fields = new List<EmbedFieldBuilder>();
                if (teamsSize < 1)
                {
                    teamsSize = users.Count / teamsCount;
                    var teams = new string[teamsCount];
                    for (int i = 0; i < users.Count; i++)
                    {
                        teams[i % teamsCount] += users[i].Mention + " ";
                    }
                    for (int i = 0; i < teamsCount; i++)
                    {
                        fields.Add(new EmbedFieldBuilder().WithName($"Team_{i + 1}:").WithValue(teams[i]));
                    }
                }
                else
                {
                    var teams = new string[teamsCount];
                    for (int i = 0; i < teamsCount * teamsSize; i++)
                    {
                        teams[i % teamsCount] += users[i].Mention + " ";
                    }
                    for (int i = 0; i < teamsCount; i++)
                    {
                        fields.Add(new EmbedFieldBuilder().WithName($"Team_{i + 1}:").WithValue(teams[i]));
                    }
                }
                commandResult = new CommandResult().WithEmbed(new EmbedBuilder()
                    .WithAuthor("DiscordBotDurak")
                    .WithColor(Color.Blue)
                    .WithFields(fields)
                    .WithFooter("Random distribution command")
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
