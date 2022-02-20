using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotDURAK.NewBot.Functions.Fabrics
{
    class RandomNumberFabric : IFabric
    {
        public IFunction GetFunction()
        {
            return new RandomNumber();
        }
    }
}
