using DiscordBotDURAK.NewBot.Data.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotDURAK.NewBot.Functions.Fabrics
{
    class FavoritesFabric : IFabric
    {
        private CRUD database;

        public FavoritesFabric(CRUD database)
        {
            this.database = database;
        }

        public IFunction GetFunction()
        {
            return new FavoritesFunction(database);
        }
    }
}
