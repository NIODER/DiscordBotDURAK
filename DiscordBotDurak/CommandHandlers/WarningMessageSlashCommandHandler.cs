using Discord.WebSocket;
using DiscordBotDurak.Commands;
using System.Linq;

namespace DiscordBotDurak.CommandHandlers
{
    internal class WarningMessageSlashCommandHandler : ICommandHandler
    {
        private readonly ulong _channelId;
        private readonly string _message;

        public WarningMessageSlashCommandHandler(SocketSlashCommand command)
        {
            _channelId = command.ChannelId.Value;
            _message = (string)command.Data.Options.FirstOrDefault(op => op.Name == "message")?.Value;
        }

        public ICommand CreateCommand()
        {
            return new WarningMessageCommand(_channelId, _message);
        }
    }
}
