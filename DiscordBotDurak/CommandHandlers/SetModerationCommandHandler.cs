using Discord.WebSocket;
using DiscordBotDurak.Commands;
using DiscordBotDurak.Enum.ModerationModes;
using System;
using System.Linq;

namespace DiscordBotDurak.CommandHandlers
{
    internal class SetModerationCommandHandler : ICommandHandler
    {
        private readonly ulong _channelId;
        private readonly ModerationMode _moderationMode;
        private readonly ulong _listId;

        public SetModerationCommandHandler(SocketSlashCommand command)
        {
            _channelId = command.ChannelId.Value;
            _moderationMode = (ModerationMode)(long)command.Data.Options.First(op => op.Name == "moderation").Value;
            _listId = Convert.ToUInt64(command.Data.Options.First(op => op.Name == "list-id").Value);
        }

        public ICommand CreateCommand()
        {
            return new SetModerationCommand(_channelId, _moderationMode, _listId);
        }
    }
}
