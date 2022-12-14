using CyberShoke;
using CyberShoke.Objects;
using Discord;
using DiscordBotDurak.Enums;

namespace DiscordBotDurak.Commands
{
    public class CybershokeCommandSecond : ICommand
    {
        private readonly string value;

        public CybershokeCommandSecond(string componentFirstValue)
        {
            value = componentFirstValue;
        }

        public CommandResult GetResult()
        {
            SelectMenuBuilder selectMenuBuilder = new SelectMenuBuilder()
                       .WithMinValues(1)
                       .WithMaxValues(1);
            Server server = null;
            var cyberShoke = new CSServers();
            switch ((CybershokeCategories)System.Enum.Parse(typeof(CybershokeCategories), value))
            {
                case CybershokeCategories.DUELS2X2:
                    selectMenuBuilder
                        .WithCustomId("DUELS2X2")
                        .AddOption("ONLY MIRAGE", "only-mirage")
                        .AddOption("ONLY DUST2", "only-dust")
                        .AddOption("ALL MAPS", "all-maps");
                    break;
                case CybershokeCategories.DUELS:
                    selectMenuBuilder
                        .WithCustomId("DUELS")
                        .AddOption("ONLY MIRAGE", "only-mirage")
                        .AddOption("ONLY DUST2", "only-dust")
                        .AddOption("ALL MAPS", "all-maps");
                    break;
                case CybershokeCategories.RETAKE:
                    selectMenuBuilder
                        .WithCustomId("RETAKE")
                        .AddOption("1-3 LVL FACEIT", "easy")
                        .AddOption("8-10 LVL FACEIT", "hard")
                        .AddOption("9 SLOTS", "9slots")
                        .AddOption("7 SLOTS", "7slots");
                    break;
                case CybershokeCategories.RETAKECLASSIC:
                    selectMenuBuilder
                        .WithCustomId("RETAKECLASSIC")
                        .AddOption("1-3 LVL FACEIT", "easy")
                        .AddOption("4-7 LVL FACEIT", "middle")
                        .AddOption("8-10 LVL FACEIT", "hard")
                        .AddOption("OPEN TO ALL - 9 SLOTS", "9slots")
                        .AddOption("OPEN TO ALL - 7 SLOTS", "7slots");
                    break;
                case CybershokeCategories.DM:
                    selectMenuBuilder.WithCustomId("DM")
                        .AddOption("Hard", "hard")
                        .AddOption("Medium", "medium")
                        .AddOption("Easy", "easy");
                    break;
                case CybershokeCategories.HSDM:
                    server = cyberShoke.GetHSDM().GetRandom();
                    break;
                case CybershokeCategories.PISTOLDM:
                    selectMenuBuilder.WithCustomId("PISTOLDM")
                        .AddOption("PISTOL HSDM", "hsdm")
                        .AddOption("PISTOLDM MEDIUM", "medium");
                    break;
                case CybershokeCategories.MULTICFGDM:
                    server = cyberShoke.GetMULTICFGDM().GetRandom();
                    break;
                case CybershokeCategories.AWPDM:
                    server = cyberShoke.GetAWPDM().GetRandom();
                    break;
                case CybershokeCategories.AIMDM:
                    selectMenuBuilder.WithCustomId("AIMDM")
                        .AddOption("AIMDM", "classic")
                        .AddOption("PISTOL AIMDM", "pistol");
                    break;
                case CybershokeCategories.EXECUTE:
                    server = cyberShoke.GetEXECUTE().GetRandom();
                    break;
                case CybershokeCategories.PISTOLRETAKE:
                    server = cyberShoke.GetPISTOLRETAKE().GetRandom();
                    break;
                case CybershokeCategories.SURF:
                    selectMenuBuilder.WithCustomId("SURF")
                        .AddOption("TIER 1 - BEGINNER", "beginner")
                        .AddOption("TIER 1-2 - EASY", "easy")
                        .AddOption("TIER 1-3 - NORMAL", "normal")
                        .AddOption("TIER 3-4 - MEDIUM", "medium")
                        .AddOption("TIER 3-5 - HARD", "hard")
                        .AddOption("TIER 4-8 - TOP 350", "top");
                    break;
                case CybershokeCategories.BHOP:
                    selectMenuBuilder.WithCustomId("BHOP")
                        .AddOption("TIER 1-2 - EASY", "easy")
                        .AddOption("TIER 3-4 - MEDIUM", "medium")
                        .AddOption("TIER 5-6 - HARD", "hard")
                        .AddOption("LEGENDARY MAPS", "legendary");
                    break;
                case CybershokeCategories.KZ:
                    selectMenuBuilder.WithCustomId("KZ")
                        .AddOption("GOKZ - TIER 1-2", "go-easy")
                        .AddOption("GOKZ - TIER 3-4", "go-middle")
                        .AddOption("GOKZ - TIER 5-6", "go-hard");
                    break;
                case CybershokeCategories.ARENA:
                    server = cyberShoke.GetARENA().GetRandom();
                    break;
                case CybershokeCategories.PUBLIC:
                    selectMenuBuilder.WithCustomId("PUBLIC")
                        .AddOption("ONLY DUST2", "only-dust")
                        .AddOption("ONLY MIRAGE", "only-mirage")
                        .AddOption("TRENDING", "trending")
                        .AddOption("WH ON", "wh")
                        .AddOption("ALL MAPS", "all-maps");
                    break;
                case CybershokeCategories.AWP:
                    selectMenuBuilder.WithCustomId("AWP")
                        .AddOption("AWP CANNONS", "cannons")
                        .AddOption("ONLY AWP LEGO 2", "lego")
                        .AddOption("AWP SERVERS", "servers");
                    break;
                case CybershokeCategories.MANIAC:
                    server = cyberShoke.GetMANIAC().GetRandom();
                    break;
                case CybershokeCategories.PROPHUNT:
                    server = cyberShoke.GetPROPHUNT().GetRandom();
                    break;
                case CybershokeCategories.HNS:
                    selectMenuBuilder.WithCustomId("HNS")
                        .AddOption("HNS SERVERS", "servers")
                        .AddOption("HNS NO RULES", "no-rules")
                        .AddOption("HNS TRAINING", "training");
                    break;
                case CybershokeCategories.KNIFE:
                    server = cyberShoke.GetKNIFE().GetRandom();
                    break;
                default:
                    break;
            }
            if (server is null)
            {
                return new CommandResult().WithMessageComponent(new ComponentBuilder().WithSelectMenu(selectMenuBuilder).Build());
            }
            else
            {
                return new CommandResult()
                    .WithEmbed(new EmbedBuilder()
                        .WithTitle("Server")
                        .WithColor(Color.Blue)
                        .AddField("ip:", $"connect {server.ip}:{server.port}")
                        .AddField("fast connect:", $"steam://connect/{server.ip}:{server.port}")
                        .WithFooter("Cybershoke command")
                        .WithCurrentTimestamp());
            }
        }
    }
}
