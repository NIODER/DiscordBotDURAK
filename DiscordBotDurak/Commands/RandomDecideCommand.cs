using Discord;
using System;

namespace DiscordBotDurak.Commands
{
    public class RandomDecideCommand : ICommand
    {
        public CommandResult GetResult() => new CommandResult()
                .WithEmbed(new EmbedBuilder()
                    .WithAuthor("DiscordBotDurak")
                    .WithColor(Color.Blue)
                    .AddField("Result:", new Random().Next(9) % 2 == 0 ? "yes" : "no")
                    .WithFooter("Random decide command")
                    .WithCurrentTimestamp());
    }
}
