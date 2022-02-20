using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotDURAK.NewBot.Functions
{
    class RandomNumber : IFunction
    {
        public object Execute(params object[] inputs)
        {
            int random;
            if (inputs == null)
            {
                random = new Random().Next(1000);
            }
            else
            {
                try
                {
                    int border = (int)inputs[0];
                    random = new Random().Next(border);
                }
                catch (InvalidCastException e)
                {
                    _ = new Log(new Discord.LogMessage(Discord.LogSeverity.Error, "RandomNumber", "CastException", e));
                    return "Что-то пошло не так";
                }
            }
            return "Артему сегодня повезло, выпало число " + new Random().Next(random);
        }
    }
}
