using Discord.WebSocket;
using DiscordBotDurak.Commands;
using System.Linq;

namespace DiscordBotDurak.CommandHandlers
{
    internal class InfoSlashCommandHandler : ICommandHandler
    {
        public enum Type
        {
            Guild,
            Channel,
            User
        }

        private readonly Type _type;
        private readonly SocketGuildUser _user;
        private readonly SocketChannel _channel;
        private readonly SocketGuild _guild;

        public InfoSlashCommandHandler(SocketSlashCommand command)
        {
            var guildCommand = command.Data.Options.FirstOrDefault(op => op.Name == "guild");
            var channelCommand = command.Data.Options.FirstOrDefault(op => op.Name == "channel");
            var userCommand = command.Data.Options.FirstOrDefault(op => op.Name == "user");
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
            else
            {
                _type = Type.User;
                _user = (SocketGuildUser)userCommand.Options.First(op => op.Name == "user-mention").Value;
            }
        }

        public ICommand CreateCommand()
        {
            return new InfoCommand(_type, _user, _channel, _guild);
        }
    }
}
