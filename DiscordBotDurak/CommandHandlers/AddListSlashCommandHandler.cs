using Discord.WebSocket;
using DiscordBotDurak.Commands;
using DiscordBotDurak.Enum.ModerationModes;
using DiscordBotDurak.Verification;
using System;
using System.Linq;

namespace DiscordBotDurak.CommandHandlers
{
    internal class AddListSlashCommandHandler : AvailableToAdmin, ICommandHandler
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
            var opt = command.Data.Options.FirstOrDefault(op => op.Name == "list-id")?.Value;
            if (opt is not null)
            {
                try
                {
                    _list = Convert.ToUInt64(opt);
                }
                catch (OverflowException e)
                {
                    _list = 0;
                    Logger.Log(Discord.LogSeverity.Info, GetType().Name, "_list overflow exception.", e);
                }
            }
            _moderationMode = (ModerationMode?)(long?)(command.Data.Options.FirstOrDefault(op => op.Name == "moderation")?.Value);
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
