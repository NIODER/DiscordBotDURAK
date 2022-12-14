using Discord;
using Discord.WebSocket;
using DiscordBotDurak.Enum.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBotDurak.Commands
{
    public class SpamCommand : LastingCommand, ICommand
    {
        private readonly long count;
        private readonly string content;
        private readonly ISocketMessageChannel channel;
        private readonly Exception exception;
        private readonly SocketSlashCommand slashCommand;

        public SpamCommand(
            ulong authorId,
            ulong guildId,
            long count,
            string content,
            ISocketMessageChannel channel,
            Exception exception,
            SocketSlashCommand slashCommand) : base(authorId, guildId, CommandType.Spam)
        {
            this.count = count;
            this.content = content;
            this.channel = channel;
            this.exception = exception;
            this.slashCommand = slashCommand;
        }

        public CommandResult GetResult()
        {
            CommandResult commandResult = null;
            if (exception is null)
            {
                commandResult = new CommandResult().WithText("Working...");
                var task = Task.Run(async () =>
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (CancellationToken.IsCancellationRequested) return;
                        await channel.SendMessageAsync(content);
                    }
                    _ = slashCommand.ModifyOriginalResponseAsync(msg => msg.Content = "Completed.");
                }, CancellationToken);
                _ = Logger.Instance().LogAsync(new LogMessage(LogSeverity.Info, GetType().Name, "Success."));
            }
            else commandResult = new CommandResult().WithException(exception);

            return commandResult;
        }
    }
}
