using DiscordBotDURAK.NewBot.Data;
using DiscordBotDURAK.NewBot.Data.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotDURAK.NewBot.Functions.Fabrics.Validation
{
    class Validator
    {
        private IFabric fabric;

        public IFabric Fabric { get => fabric; }

        public Validator(string message, CRUD database)
        {
            switch (message.Split("")[0])
            {
                case "$radio":
                    fabric = new RadioFabric();
                    break;
                case "$rand":
                    fabric = new RandomNumberFabric();
                    break;
                case "$joke":
                    fabric = new JokeFabric();
                    break;
                case "$surf":
                    fabric = new SurfFabric();
                    break;
                case "$help":
                    fabric = new HelpFabric();
                    break;
                case "реши":
                    fabric = new DesideFabric();
                    break;
                case "$избранное":
                    fabric = new FavoritesFabric(database);
                    break;
                default:
                    if (message.Contains("?") || message.Contains("бот"))
                    {
                        fabric = new BalabobaFabric();
                    }
                    break;
            }
        }
    }
}
