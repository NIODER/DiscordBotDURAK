using Discord.WebSocket;
using DiscordBotDurak.Commands;
using DiscordBotDurak.Exceptions;

namespace DiscordBotDurak.CommandHandlers
{
    public class RemoderateCommandHandler : ICommandHandler
    {
        private readonly SocketGuildChannel channel;
        private readonly ulong authorId;
        private readonly DiscordBotSlashCommandException exception;

        public RemoderateCommandHandler(SocketSlashCommand command)
        {
            if (command.Channel is ISocketMessageChannel and SocketGuildChannel guildChannel)
            {
                channel = guildChannel;
                authorId = command.User.Id;
            }
            else exception = new("remoderate", "Command executed not in message guild channel.");
        }

        public ICommand CreateCommand() => exception is null
                ? new RemoderateCommand(channel, authorId)
                : new RemoderateCommand(exception);
    }
}
