using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotDurak.Commands
{
    internal class HelpCommand : ICommand
    {
        private readonly long _variant;

        public HelpCommand(long variant)
        {
            _variant = variant;
        }

        public CommandResult GetResult() => _variant switch
        {
            0 => new CommandResult().WithEmbed(new EmbedBuilder()
                .AddField("Spy help message", new Constants().GetSpyHelpMessage())
                .WithColor(Color.LightGrey)
                .WithCurrentTimestamp()),
            1 => new CommandResult().WithEmbed(new EmbedBuilder()
                .AddField("Forbidden symbols message", new Constants().GetSymbolsListsHelpMessage1())
                .AddField("Symbols parameters", new Constants().GetSymbolsListsHelpMessage2())
                .WithColor(Color.LightGrey)
                .WithCurrentTimestamp()),
            2 => new CommandResult().WithEmbed(new EmbedBuilder()
                .AddField("Users help message", new Constants().GetUserHelpMessage())),
            _ => new CommandResult().WithException("Something went wrong")
        };
    }
}
