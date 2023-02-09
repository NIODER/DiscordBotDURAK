using Discord.WebSocket;
using DiscordBotDurak.Commands;
using DiscordBotDurak.Data;
using DiscordBotDurak.Enum.BotRoles;
using System.Linq;

namespace DiscordBotDurak.CommandHandlers
{
    internal class MailingListSlashCommandHandler : IVerifiable, ICommandHandler
    {
        private readonly SocketGuildUser _user;
        private readonly MailingAction _action;

        public MailingListSlashCommandHandler(SocketSlashCommand command)
        {
            _action = (MailingAction)(long)command.Data.Options.First(op => op.Name == "action").Value;
            _user = (SocketGuildUser)command.Data.Options.FirstOrDefault(op => op.Name == "user")?.Value;
        }

        public ICommand CreateCommand()
        {
            return new MailingListCommand(_user, _action);
        }

        public bool Verify(ulong userId, ulong guildId)
        {
            using var db = new Database();
            var user = db.GetUser(guildId, userId);
            if (_user.Id == userId && _action == MailingAction.Delete)
            {
                return true;
            }
            return user.Role >= (short)BotRole.Admin;
        }
    }
}
