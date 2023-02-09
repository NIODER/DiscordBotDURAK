using DiscordBotDurak.Commands;
using DiscordBotDurak.Verification;

namespace DiscordBotDurak.CommandHandlers
{
    internal class RandomDecideCommandHandler : AvailableToEveryone, ICommandHandler
    {
        public ICommand CreateCommand() => new RandomDecideCommand();
    }
}
