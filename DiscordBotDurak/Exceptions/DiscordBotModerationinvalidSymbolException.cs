using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotDurak.Exceptions
{
    public class DiscordBotModerationinvalidSymbolException : DiscordBotModerationException
    {
        public DiscordBotModerationinvalidSymbolException() : base()
        {
        }

        public DiscordBotModerationinvalidSymbolException(string message) : base(message)
        {
        }
    }
}
