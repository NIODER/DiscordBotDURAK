using Discord.WebSocket;
using DiscordBotDurak.Commands;
using DiscordBotDurak.Verification;
using System.Linq;

namespace DiscordBotDurak.CommandHandlers
{
    internal class GetSymbolsListsSlashCommandHandler : AvailableToAdmin, ICommandHandler
    {
        private readonly long _scope;
        private readonly ulong _guildId;
        private readonly ulong _channelId;

        public GetSymbolsListsSlashCommandHandler(SocketSlashCommand command)
        {
            _scope = (long)command.Data.Options.First(op => op.Name == "scope").Value;
            _channelId = command.ChannelId.Value;
            _guildId = command.GuildId.Value;
        }

        public ICommand CreateCommand()
        {
            return new GetSymbolsListsCommand(_channelId, _guildId, _scope);
        }
    }
}
