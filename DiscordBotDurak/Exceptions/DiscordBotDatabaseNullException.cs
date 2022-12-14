using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotDurak.Exceptions
{
    public class DiscordBotDatabaseNullException : DiscordBotDatabaseException
    {
        public string NulledProperty { get; private set; }

        public DiscordBotDatabaseNullException(CRUD action) : base(action)
        {
            NulledProperty = "unknown";
        }

        public DiscordBotDatabaseNullException(CRUD action, string message) : base(action, message)
        {
            NulledProperty = "unknown";
        }
        public DiscordBotDatabaseNullException(CRUD action, string property, string message) : base(action, message)
        {
            NulledProperty = property;
        }
        public override string ToString()
        {
            return $"{base.ToString()}\n" +
                $"Null occured on property: {NulledProperty}.";
        }
    }
}
