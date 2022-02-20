using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotDURAK.NewBot.Functions.Fabrics
{
    class JokeFabric : IFabric
    {
        public IFunction GetFunction()
        {
            return new Joke();
        }
    }
}
