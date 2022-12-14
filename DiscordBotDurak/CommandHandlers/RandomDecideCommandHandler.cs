using DiscordBotDurak.Commands;

namespace DiscordBotDurak.CommandHandlers
{
    public class RandomDecideCommandHandler : ICommandHandler
    {
        public ICommand CreateCommand() => new RandomDecideCommand();
    }
}
