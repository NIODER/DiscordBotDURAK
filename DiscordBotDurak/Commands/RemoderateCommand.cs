using Discord.WebSocket;
using DiscordBotDurak.Exceptions;
using System.Threading.Tasks;

namespace DiscordBotDurak.Commands
{
    public class RemoderateCommand : LastingCommand, ICommand
    {
        private readonly SocketGuildChannel channel;
        private readonly DiscordBotSlashCommandException _exception;

        public RemoderateCommand(SocketGuildChannel channel, 
            ulong authorId) 
            : base(authorId, channel.Guild.Id, Enum.Commands.CommandType.Remoderate)
        {
            this.channel = channel;
        }

        public RemoderateCommand(DiscordBotSlashCommandException exception)
        {
            _exception = exception;
        }

        private async Task RemoderateAsync()
        {
            await foreach (var messages in ((ISocketMessageChannel)channel).GetMessagesAsync())
                foreach (var message in messages)
                    if (!CancellationToken.IsCancellationRequested)
                        Moderator.Moderate(message, channel, false);
        }

        public CommandResult GetResult()
        {
            if (_exception == null)
                _ = RemoderateAsync();
            else return new CommandResult().WithException(_exception);
            return new CommandResult().WithText("Remoderation started.");
        }
    }
}
