using Discord.WebSocket;
using DiscordBotDurak.Commands;
using DiscordBotDurak.Enum.Commands;
using System.Linq;

namespace DiscordBotDurak.CommandHandlers
{
    public class DeleteCommandHandler : ICommandHandler
    {
        private readonly ISocketMessageChannel channel;
        private readonly long type;
        private readonly string content;
        private readonly ulong userId;
        private readonly ulong guildId;
        private readonly CommandType commandType;

        public DeleteCommandHandler(SocketSlashCommand command) 
        {
            channel = command.Channel;
            type = (long)command.Data.Options.First(op => op.Name == "type").Value;
            content = (string)command.Data.Options.FirstOrDefault(op => op.Name == "content").Value;
            userId = command.User.Id;
            guildId = (ulong)command.GuildId;
            commandType = CommandType.Delete;
        }

        public ICommand CreateCommand()
        {
            return new DeleteCommand(type, channel, content, userId, guildId, commandType);
        }
    }
}
