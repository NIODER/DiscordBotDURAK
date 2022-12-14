using System;

namespace DiscordBotDurak.Exceptions
{
    public class DiscordBotSlashCommandException : DiscordBotException
    {
        private const string src = "Command";
        public string CommandName { get; private set; }

        public DiscordBotSlashCommandException(string commandName) : base(src)
        {
            Source = "SlashCommand";
            CommandName = commandName;
        }

        public DiscordBotSlashCommandException(string commandName, string message) : base(message, src)
        {
            Source = "SlashCommand";
            CommandName = commandName;
        }

        public override string ToString()
        {
            return $"{Source}, {CommandName}.\n{base.ToString()}";
        }
    }
}
