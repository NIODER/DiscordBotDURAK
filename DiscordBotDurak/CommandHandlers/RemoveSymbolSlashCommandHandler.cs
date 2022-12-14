using Discord.WebSocket;
using DiscordBotDurak.Commands;
using System;
using System.Linq;

namespace DiscordBotDurak.CommandHandlers
{
    internal class RemoveSymbolSlashCommandHandler : ICommandHandler
    {
        private readonly ulong _symbolId;
        private readonly string _lists;
        private readonly ulong _guildId;

        public RemoveSymbolSlashCommandHandler(SocketSlashCommand command)
        {
            _symbolId = Convert.ToUInt64(command.Data.Options.First(op => op.Name == "symbol-id").Value);
            _lists = (string)command.Data.Options.First(op => op.Name == "lists").Value;
            _guildId = command.GuildId ?? 0;
        }

        public ICommand CreateCommand()
        {
            return new RemoveSymbolCommand(_lists, _symbolId, _guildId);
        }
    }
}
