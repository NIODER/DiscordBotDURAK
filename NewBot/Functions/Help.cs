using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotDURAK.NewBot.Functions
{
    class Help : IFunction
    {
        public object Execute(params object[] inputs)
        {
            StreamReader reader = new(File.OpenRead(@"\NewBot\Data\Help_message.txt"));
            return reader.ReadToEnd();
        }
    }
}
