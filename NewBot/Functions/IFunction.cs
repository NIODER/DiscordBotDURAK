using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotDURAK.NewBot.Functions
{
    interface IFunction
    {
        abstract object Execute(params object[] inputs);
    }
}
