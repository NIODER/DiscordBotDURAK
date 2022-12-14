using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotDurak.Exceptions
{
    public class DiscordBotDatabaseException : DiscordBotException
    {
        public enum CRUD
        {
            Create,
            Read,
            Update,
            Delete
        }

        private const string src = "Database";

        public CRUD Action { get; private set; }

        public DiscordBotDatabaseException(CRUD action) : base(src)
        {
            Action = action;
        }

        public DiscordBotDatabaseException(CRUD action, string message) : base(message, src)
        {
            Action = action;
        }

        public override string ToString()
        {
            return $"{base.ToString()}\nException occured on {Action}.";
        }
    }
}
