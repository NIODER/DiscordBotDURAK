using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordBotDURAK.NewBot.Data;

namespace DiscordBotDURAK.NewBot.Functions
{
    class FavoritesFunction : IFunction
    {
        private Database database;

        public FavoritesFunction(CRUD database)
        {
            this.database = database;
        }

        public object Execute(params object[] inputs)
        {
            List<string> messageContents = (List<string>)inputs[0];

            database.Read(Actions.Radio, )
        }
    }
}
