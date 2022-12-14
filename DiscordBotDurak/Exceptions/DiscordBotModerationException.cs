using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotDurak.Exceptions
{
    public class DiscordBotModerationException : DiscordBotException
    {
        protected const string src = "Moderation";

        public DiscordBotModerationException() : base(src)
        {

        }

        public DiscordBotModerationException(string message) : base(message, src)
        {

        }
    }
}
