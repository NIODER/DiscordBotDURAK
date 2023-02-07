using Discord.WebSocket;
using DiscordBotDurak.Commands;
using System;
using System.Linq;

namespace DiscordBotDurak.CommandHandlers
{
    internal class InfoSlashCommandHandler : ICommandHandler
    {
        public enum Type
        {
            Guild,
            Channel,
            List,
            User
        }

        private readonly Type _type;
        private readonly SocketGuildUser _user;
        private readonly SocketChannel _channel;
        private readonly SocketGuild _guild;
        private readonly ulong _listId;

        public InfoSlashCommandHandler(SocketSlashCommand command)
        {
            var guildCommand = command.Data.Options.FirstOrDefault(op => op.Name == "guild");
            var channelCommand = command.Data.Options.FirstOrDefault(op => op.Name == "channel");
            var userCommand = command.Data.Options.FirstOrDefault(op => op.Name == "user");
            var listCommand = command.Data.Options.FirstOrDefault(op => op.Name == "list");
            _guild = ((SocketGuildChannel)command.Channel).Guild;
            if (guildCommand is not null)
            {
                _type = Type.Guild;
            }
            else if (channelCommand is not null)
            {
                _type = Type.Channel;
                _channel = (SocketChannel)channelCommand.Options.First(op => op.Name == "channel-mention").Value;
            }
            else if (userCommand is not null)
            {
                _type = Type.User;
                _user = (SocketGuildUser)userCommand.Options.First(op => op.Name == "user-mention").Value;
            }
            else
            {
                _type = Type.List;
                try
                {
                    _listId = Convert.ToUInt64(listCommand.Options.First(op => op.Name == "list-id").Value);
                }
                catch (OverflowException e)
                {
                    _listId = 0;
                    Logger.Log(Discord.LogSeverity.Info, GetType().Name, "_listId overflow exception.", e);
                }
            }
        }

        public ICommand CreateCommand()
        {
            return new InfoCommand(_type, _user, _channel, _guild, _listId);
        }
    }
}
