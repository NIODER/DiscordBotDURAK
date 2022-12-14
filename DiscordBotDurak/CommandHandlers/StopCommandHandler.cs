using Discord.WebSocket;
using DiscordBotDurak.Commands;
using DiscordBotDurak.Enum.Commands;
using System.Linq;

namespace DiscordBotDurak.CommandHandlers
{
    public class StopCommandHandler : ICommandHandler
    {
        private readonly CommandType commandType;
        private readonly ulong authorId;
        private readonly ulong guildId;

        public StopCommandHandler(SocketSlashCommand command)
        {
            commandType = (CommandType)(long)command.Data.Options.First(option => option.Name == "command").Value;
            authorId = command.User.Id;
            guildId = (command.Channel as SocketGuildChannel).Guild.Id;
        }

        public ICommand CreateCommand() => new StopCommand(commandType, authorId, guildId);
    }
}
