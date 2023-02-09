using Discord.WebSocket;
using DiscordBotDurak.Commands;
using DiscordBotDurak.Verification;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscordBotDurak.CommandHandlers
{
    internal class RemoveListSlashCommandHandler : AvailableToAdmin, ICommandHandler
    {
        private readonly int _scope;
        private readonly string _lists;
        private readonly List<ulong> _channelIds;
        private readonly ulong _guildId;

        public RemoveListSlashCommandHandler(SocketSlashCommand command)
        {
            _scope = Convert.ToInt32(command.Data.Options.First(op => op.Name == "scope").Value);
            _lists = (string)command.Data.Options.First(op => op.Name == "lists");
            if (_scope == 0)
            {
                _channelIds = (command.Channel as SocketGuildChannel).Guild.TextChannels.Select(c => c.Id).ToList();
            }
            else
            {
                _channelIds = new List<ulong>()
                {
                    command.ChannelId.Value
                };
            }
            _guildId = command.GuildId.Value;
        }

        public ICommand CreateCommand()
        {
            return new RemoveListCommand(_lists, _scope, _guildId, _channelIds);
        }
    }
}
