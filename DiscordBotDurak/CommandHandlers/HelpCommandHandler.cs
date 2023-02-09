using Discord.WebSocket;
using DiscordBotDurak.Commands;
using DiscordBotDurak.Verification;
using System.Linq;

namespace DiscordBotDurak.CommandHandlers
{
    internal class HelpCommandHandler : AvailableToEveryone, ICommandHandler
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
