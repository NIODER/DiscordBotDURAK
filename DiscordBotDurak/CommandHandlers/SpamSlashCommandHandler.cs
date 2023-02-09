using Discord.WebSocket;
using DiscordBotDurak.Commands;
using DiscordBotDurak.Exceptions;
using DiscordBotDurak.Verification;
using System;
using System.Linq;

namespace DiscordBotDurak.CommandHandlers
{
    internal class SpamSlashCommandHandler : AvailableToModerator, ICommandHandler
    {
        private readonly long count;
        private readonly string content;
        private readonly ISocketMessageChannel channel;
        private readonly Exception exception;
        private readonly ulong authorId;
        private readonly ulong guildId;
        private readonly SocketSlashCommand slashCommand;

        public SpamSlashCommandHandler(SocketSlashCommand slashCommand)
        {
            this.slashCommand = slashCommand;
            exception = null;
            count = (long)slashCommand.Data.Options.Where(option => option.Name == "count").First().Value;
            content = (string)slashCommand.Data.Options.Where(option => option.Name == "message").First().Value;
            channel = slashCommand.Channel;
            authorId = slashCommand.User.Id;
            if (slashCommand.Channel is SocketGuildChannel guildChannel)
                guildId = guildChannel.Guild.Id;
            else exception = new DiscordBotSlashCommandException(slashCommand.CommandName, "Command executed not in guild");
            if (Moderator.ContainsBanword(channel.Id, content) != 0)
                exception = new DiscordBotModerationException("The message contains a forbidden character");
        }

        public ICommand CreateCommand() => new SpamCommand(authorId, guildId, count, content, channel, exception, slashCommand);
    }
}
