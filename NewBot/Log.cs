using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotDURAK.NewBot
{
    public class Log
    {
        public Log(LogMessage logMessage)
        {
            Console.WriteLine(logMessage.ToString());
        }
    }
}
