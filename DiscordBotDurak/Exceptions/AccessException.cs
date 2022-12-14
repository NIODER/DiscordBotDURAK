using DatabaseModel;
using DiscordBotDurak.Data;

namespace DiscordBotDurak.Exceptions
{
    public class AccessException : CommandException
    {
        private readonly GuildUser user;

        public AccessException(ICommand command, GuildUser user) : base(command)
        {
            this.user = user;
        }

        public AccessException(ICommand command, GuildUser user, string message) : base(command, message)
        {
            this.user = user;
        }

        public override string ToString()
        {
            return $"User has no access to this command.\n" +
                $"User:\n\tid: {user.UserId}.\n\tguildId: {user.GuildId}.\n" +
                $"{base.ToString()}";
        }
    }
}
