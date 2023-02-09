using Discord.WebSocket;
using DiscordBotDurak.Commands;
using DiscordBotDurak.Verification;
using System.Linq;

namespace DiscordBotDurak.CommandHandlers
{
    internal class BaseRoleSlashCommandHandler : AvailableToAdmin, ICommandHandler
    {
        private readonly SocketGuild _guild;
        private readonly SocketRole _role;

        public BaseRoleSlashCommandHandler(SocketSlashCommand command)
        {
            _guild = (command.Channel as SocketGuildChannel).Guild;
            _role = command.Data.Options.FirstOrDefault(op => op.Name == "role")?.Value as SocketRole;
        }

        public ICommand CreateCommand()
        {
            return new BaseRoleCommand(_guild, _role);
        }
    }
}
