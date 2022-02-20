using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotDURAK.NewBot.Data
{
    public enum ChannelSeverity
    {
        /// <summary>
        /// Bot does not have access
        /// to this channel
        /// </summary>
        None = 0,
        /// <summary>
        /// Channel with rules of guild
        /// </summary>
        Welcome = 1,
        /// <summary>
        /// Channel with references
        /// </summary>
        References = 2,
        /// <summary>
        /// Channel with flood
        /// </summary>
        Flood = 3,
        NoSuchChannel = 4
    }
}
