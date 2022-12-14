using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotDurak.Exceptions
{
    public class DiscordBotModerationGuildChannelException : DiscordBotModerationException
    {
        public ulong ChannelId { get; set; }
        public ulong GuildId { get; set; }

        public DiscordBotModerationGuildChannelException() : base()
        {
        }

        public DiscordBotModerationGuildChannelException(string message) : base(message)
        {
        }
    }
}
