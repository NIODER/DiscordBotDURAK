namespace DiscordBotDurak.Exceptions
{
    public class CommandException : DiscordBotException
    {
        private const string src = "Command";
        private readonly string commandName;

        public CommandException(ICommand command) : base(src)
        {
            commandName = command.GetType().Name;
        }

        public CommandException(ICommand command, string message) : base(message, src)
        {
            commandName = command.GetType().Name;
        }

        public override string ToString()
        {
            return $"Command: {commandName}.\n" +
                $"{base.ToString()}";
        }
    }
}
