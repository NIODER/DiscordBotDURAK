using Discord.WebSocket;
using DiscordBotDurak.Commands;
using System;
using System.Linq;

namespace DiscordBotDurak.CommandHandlers
{
    internal class GetSymbolsSlashCommandHandler : ICommandHandler
    {
        private readonly string lists;

        public GetSymbolsSlashCommandHandler(SocketSlashCommand command)
        {
            lists = (string)command.Data.Options.First(op => op.Name == "lists");
        }

        public ICommand CreateCommand()
        {
            return new GetSymbolsCommand(lists);
        }
    }
}
