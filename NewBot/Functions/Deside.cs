using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotDURAK.NewBot.Functions
{
    class Deside : IFunction
    {
        public object Execute(params object[] inputs)
        {
            Random random = new Random();
            if (random.Next(10) % 2 == 0)
            {
                return "да";
            }
            else
            {
                return "нет";
            }
        }
    }
}
