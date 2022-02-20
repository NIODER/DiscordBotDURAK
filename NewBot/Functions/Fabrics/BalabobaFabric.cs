using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotDURAK.NewBot.Functions.Fabrics
{
    class BalabobaFabric : IFabric
    {
        public IFunction GetFunction()
        {
            return new Balaboba.Balaboba();
        }
    }
}
