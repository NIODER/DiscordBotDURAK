using Discord.WebSocket;
using DiscordBotDurak.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotDurak.CommandHandlers
{
    internal class HelpCommandHandler : ICommandHandler
    {
        private readonly long _variant;

        public HelpCommandHandler(SocketSlashCommand command)
        {
            _variant = (long)command.Data.Options.First(op => op.Name == "type").Value;
        }

        public ICommand CreateCommand()
        {
            return new HelpCommand(_variant);                
        }
    }
}
