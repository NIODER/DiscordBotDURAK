using Discord.WebSocket;
using DiscordBotDurak.Commands;
using DiscordBotDurak.Enum.ModerationModes;
using System;
using System.Linq;

namespace DiscordBotDurak.CommandHandlers
{
    internal class AddListSlashCommandHandler : ICommandHandler
    {
        private readonly long _scope;
        private readonly ulong? _list;
        private readonly ModerationMode? _moderationMode;
        private readonly string _title;
        private readonly ulong _guildId;
        private readonly ulong _channelId;
        private readonly ulong? _resendChannelId;

        public AddListSlashCommandHandler(SocketSlashCommand command)
        {
            _scope = (long)command.Data.Options.First(op => op.Name == "scope").Value;
            _list = (ulong?)command.Data.Options.FirstOrDefault(op => op.Name == "list-id")?.Value;
            _moderationMode = (ModerationMode?)(long?)command.Data.Options.FirstOrDefault(op => op.Name == "moderation")?.Value;
            _title = (string)command.Data.Options.FirstOrDefault(op => op.Name == "title")?.Value;
            _resendChannelId = ((SocketChannel)command.Data.Options.FirstOrDefault(op => op.Name == "resend-channel")?.Value)?.Id;
            _guildId = command.GuildId ?? 0;
            _channelId = command.ChannelId ?? 0;
        }

        public ICommand CreateCommand()
        {
            return new AddListCommand(_scope, _list, _moderationMode, _title, _guildId, _channelId, _resendChannelId);
        }
    }
}
