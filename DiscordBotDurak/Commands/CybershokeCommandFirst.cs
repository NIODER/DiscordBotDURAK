using CyberShoke;
using Discord;

namespace DiscordBotDurak.Commands
{
    class CybershokeCommandFirst : ICommand
    {
        public CommandResult GetResult()
        {
            SelectMenuBuilder selectMenuBuilder = new SelectMenuBuilder()
                .WithPlaceholder("Выберите режим")
                .WithCustomId("cs-main")
                .WithMinValues(1)
                .WithMaxValues(1);

            string banned = "AMONGUS MINIGAMES SURFCOMBAT JAIL DEATHRUN";
            var listmodes = new CSServers().Listmodes;

            foreach (var item in banned.Split(" "))
            {
                listmodes.Remove(item);
            }

            foreach (var item in listmodes)
            {
                selectMenuBuilder.AddOption(label: item, value: item);
            }

            var builder = new ComponentBuilder()
                .WithSelectMenu(selectMenuBuilder);

            return new CommandResult().WithMessageComponent(builder.Build());
        }
    }
}
