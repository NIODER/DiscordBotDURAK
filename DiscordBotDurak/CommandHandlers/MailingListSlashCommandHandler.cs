using DatabaseModel;
using Discord.WebSocket;
using DiscordBotDurak.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotDurak.CommandHandlers
{
    internal class MailingListSlashCommandHandler : ICommandHandler
    {
        private readonly SocketGuildUser _user;
        private readonly MailingAction _action;

        public MailingListSlashCommandHandler(SocketSlashCommand command)
        {
            _action = (MailingAction)command.Data.Options.First(op => op.Name == "action").Value;
            _user = (SocketGuildUser)command.Data.Options.FirstOrDefault(op => op.Name == "user")?.Value;
        }

        public ICommand CreateCommand()
        {
            return new MailingListCommand(_user, _action);
        }
    }
}
