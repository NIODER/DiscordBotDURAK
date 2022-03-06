using CyberShoke;
using Discord;
using Discord.WebSocket;
using System;
using System.Linq;

namespace DiscordBotDURAK
{
    class CybershokeCSGOCommandButtons
    {
        public static (ComponentBuilder builder, string text) CsMain(SocketMessageComponent component)
        {
            SelectMenuBuilder selectMenuBuilder = new SelectMenuBuilder()
                        .WithMinValues(1)
                        .WithMaxValues(1);
            string text = null;
            var cyberShoke = new CSServers();
            switch ((CybershokeCategories)Enum.Parse(typeof(CybershokeCategories), component.Data.Values.ElementAt(0)))
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
                        .AddOption("18 SLOTS LITE 1-3LVL FACEIT", "18easy")
                        .AddOption("16 SLOTS LITE 1-3LVL FACEIT", "16easy")
                        .AddOption("14 SLOTS LITE 1-3LVL FACEIT", "14easy")
                        .AddOption("20 SLOTS LITE", "20lite")
                        .AddOption("18 SLOTS LITE", "18lite")
                        .AddOption("16 SLOTS LITE", "16lite")
                        .AddOption("18 SLOTS", "18slots")
                        .AddOption("16 SLOTS", "16slots")
                        .AddOption("NOAWP", "noawp");
                    break;
                case CybershokeCategories.HSDM:
                    selectMenuBuilder.WithCustomId("HSDM")
                        .AddOption("HSDM LITE", "lite")
                        .AddOption("HSDM", "classic")
                        .AddOption("HSDM ONETAP", "onetap");
                    break;
                case CybershokeCategories.PISTOLDM:
                    selectMenuBuilder.WithCustomId("PISTOLDM")
                        .AddOption("PISTOL HSDM", "hsdm")
                        .AddOption("PISTOLDM LITE", "lite")
                        .AddOption("PISTOLDM", "classic");
                    break;
                case CybershokeCategories.MULTICFGDM:
                    text = cyberShoke.GetMULTICFGDM().GetRandom().Info();
                    break;
                case CybershokeCategories.AWPDM:
                    selectMenuBuilder.WithCustomId("AWPDM")
                        .AddOption("AWPDM LITE", "lite")
                        .AddOption("AWPDM", "classic")
                        .AddOption("NOSCOPEDM", "noscope");
                    break;
                case CybershokeCategories.AIMDM:
                    selectMenuBuilder.WithCustomId("AIMDM")
                        .AddOption("AIMDM", "classic")
                        .AddOption("PISTOL AIMDM", "pistol");
                    break;
                case CybershokeCategories.EXECUTE:
                    text = cyberShoke.GetEXECUTE().GetRandom().Info();
                    break;
                case CybershokeCategories.PISTOLRETAKE:
                    text = cyberShoke.GetPISTOLRETAKE().GetRandom().Info();
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
                        .AddOption("LEGENDARY MAPS", "legendary")
                        .AddOption("64 TICK", "tick");
                    break;
                case CybershokeCategories.KZ:
                    selectMenuBuilder.WithCustomId("KZ")
                        .AddOption("KZTimer - TIER 1-2", "timer-easy")
                        .AddOption("GOKZ - TIER 1-2", "go-easy")
                        .AddOption("KZTimer - TIER 3-4", "timer-middle")
                        .AddOption("GOKZ - TIER 3-4", "go-middle")
                        .AddOption("KZTimer - TIER 5-6", "timer-hard")
                        .AddOption("GOKZ - TIER 5-6", "go-hard");
                    break;
                case CybershokeCategories.ARENA:
                    text = cyberShoke.GetARENA().GetRandom().Info();
                    break;
                case CybershokeCategories.PUBLIC:
                    selectMenuBuilder.WithCustomId("PUBLIC")
                        .AddOption("ONLY DUST2", "only-dust")
                        .AddOption("ONLY MIRAGE", "only-mirage")
                        .AddOption("NO LIMIT", "no-limit")
                        .AddOption("COMPETITIVE MAPS", "competitive")
                        .AddOption("WH ON", "wh")
                        .AddOption("ALL MAPS", "all-maps")
                        .AddOption("DESTRUCTIBLE INFERNO", "destr-inferno");
                    break;
                case CybershokeCategories.AWP:
                    selectMenuBuilder.WithCustomId("AWP")
                        .AddOption("AWP CANNONS", "cannons")
                        .AddOption("ONLY AWP LEGO 2", "lego")
                        .AddOption("AWP SERVERS", "servers");
                    break;
                case CybershokeCategories.MANIAC:
                    text = cyberShoke.GetMANIAC().GetRandom().Info();
                    break;
                case CybershokeCategories.PROPHUNT:
                    text = cyberShoke.GetPROPHUNT().GetRandom().Info();
                    break;
                case CybershokeCategories.HNS:
                    selectMenuBuilder.WithCustomId("HNS")
                        .AddOption("HNS SERVERS", "servers")
                        .AddOption("HNS NO RULES", "no-rules")
                        .AddOption("HNS TRAINING", "training");
                    break;
                case CybershokeCategories.KNIFE:
                    text = cyberShoke.GetKNIFE().GetRandom().Info();
                    break;
                default:
                    break;
            }

            return (new ComponentBuilder().WithSelectMenu(selectMenuBuilder), text);
        }

        public static string Duplets(SocketMessageComponent component) => component.Data.Values.ElementAt(0) switch
        {
            "only-mirage" => new CSServers().GetDUELS2X2().ONLY_MIRAGE.GetRandom().Info(),
            "only-dust" => new CSServers().GetDUELS2X2().ONLY_DUST2.GetRandom().Info(),
            "all-maps" => new CSServers().GetDUELS2X2().ALL_MAPS.GetRandom().Info(),
            _ => throw new IndexOutOfRangeException()
        };

        public static string Duels(SocketMessageComponent component) => component.Data.Values.ElementAt(0) switch
        {
            "only-mirage" => new CSServers().GetDUELS().ONLY_MIRAGE.GetRandom().Info(),
            "only-dust" => new CSServers().GetDUELS().ONLY_DUST2.GetRandom().Info(),
            "all-maps" => new CSServers().GetDUELS().ALL_MAPS.GetRandom().Info(),
            _ => throw new IndexOutOfRangeException()
        };

        public static string Retake(SocketMessageComponent component) => component.Data.Values.ElementAt(0) switch
        {
            "easy" => new CSServers().GetRETAKE().EASY.GetRandom().Info(),
            "hard" => new CSServers().GetRETAKE().HARD.GetRandom().Info(),
            "9slots" => new CSServers().GetRETAKE().SLOTS9.GetRandom().Info(),
            "7slots" => new CSServers().GetRETAKE().SLOTS7.GetRandom().Info(),
            _ => throw new IndexOutOfRangeException()
        };

        public static string RetakeClasiic(SocketMessageComponent component) => component.Data.Values.ElementAt(0) switch
        {
            "easy" => new CSServers().GetRETAKECLASSIC().EASY.GetRandom().Info(),
            "middle" => new CSServers().GetRETAKECLASSIC().MEDIUM.GetRandom().Info(),
            "hard" => new CSServers().GetRETAKECLASSIC().HARD.GetRandom().Info(),
            "9slots" => new CSServers().GetRETAKECLASSIC().SLOTS9.GetRandom().Info(),
            "7slots" => new CSServers().GetRETAKECLASSIC().SLOTS7.GetRandom().Info(),
            _ => throw new IndexOutOfRangeException()
        };

        public static string DM(SocketMessageComponent component) => component.Data.Values.ElementAt(0) switch
        {
            "18easy" => new CSServers().GetDM().EASY18.GetRandom().Info(),
            "16easy" => new CSServers().GetDM().EASY16.GetRandom().Info(),
            "14easy" => new CSServers().GetDM().EASY14.GetRandom().Info(),
            "20lite" => new CSServers().GetDM().LITE20.GetRandom().Info(),
            "18lite" => new CSServers().GetDM().LITE18.GetRandom().Info(),
            "16lite" => new CSServers().GetDM().LITE16.GetRandom().Info(),
            "18slots" => new CSServers().GetDM().SLOTS18.GetRandom().Info(),
            "16slots" => new CSServers().GetDM().SLOTS16.GetRandom().Info(),
            "noawp" => new CSServers().GetDM().NOAWP.GetRandom().Info(),
            _ => throw new IndexOutOfRangeException()
        };

        public static string HSDM(SocketMessageComponent component) => component.Data.Values.ElementAt(0) switch
        {
            "lite" => new CSServers().GetHSDM().HSDM_LITE.GetRandom().Info(),
            "classic" => new CSServers().GetHSDM().HSDM.GetRandom().Info(),
            "onetap" => new CSServers().GetHSDM().HSDM_ONETAP.GetRandom().Info(),
            _ => throw new IndexOutOfRangeException()
        };

        public static string PISTOLDM(SocketMessageComponent component) => component.Data.Values.ElementAt(0) switch
        {
            "hsdm" => new CSServers().GetPISTOLDM().PISTOL_HSDM.GetRandom().Info(),
            "lite" => new CSServers().GetPISTOLDM().PISTOLDM_LITE.GetRandom().Info(),
            "classic" => new CSServers().GetPISTOLDM().PISTOLDM.GetRandom().Info(),
            _ => throw new IndexOutOfRangeException()
        };

        public static string AWPDM(SocketMessageComponent component) => component.Data.Values.ElementAt(0) switch
        {
            "lite" => new CSServers().GetAWPDM().AWPDM_LITE.GetRandom().Info(),
            "classic" => new CSServers().GetAWPDM().AWPDM.GetRandom().Info(),
            "noscope" => new CSServers().GetAWPDM().NOSCOPEDM.GetRandom().Info(),
            _ => throw new IndexOutOfRangeException()
        };

        public static string AIMDM(SocketMessageComponent component) => component.Data.Values.ElementAt(0) switch
        {
            "classic" => new CSServers().GetAIM_DM().AIMDM.GetRandom().Info(),
            "pistol" => new CSServers().GetAIM_DM().PISTOL_AIMDM.GetRandom().Info(),
            _ => throw new IndexOutOfRangeException()
        };

        public static string SURF(SocketMessageComponent component) => component.Data.Values.ElementAt(0) switch
        {
            "beginner" => new CSServers().GetSURF().BEGINNER.GetRandom().Info(),
            "easy" => new CSServers().GetSURF().EASY.GetRandom().Info(),
            "normal" => new CSServers().GetSURF().NORMAL.GetRandom().Info(),
            "medium" => new CSServers().GetSURF().MEDIUM.GetRandom().Info(),
            "hard" => new CSServers().GetSURF().HARD.GetRandom().Info(),
            "top" => new CSServers().GetSURF().TOP.GetRandom().Info(),
            _ => throw new IndexOutOfRangeException()
        };

        public static string BHOP(SocketMessageComponent component) => component.Data.Values.ElementAt(0) switch
        {
            "easy" => new CSServers().GetBHOP().EASY.GetRandom().Info(),
            "medium" => new CSServers().GetBHOP().MEDIUM.GetRandom().Info(),
            "hard" => new CSServers().GetBHOP().HARD.GetRandom().Info(),
            "legendary" => new CSServers().GetBHOP().LEGEMDARY.GetRandom().Info(),
            "tick" => new CSServers().GetBHOP().TICK.GetRandom().Info(),
            _ => throw new IndexOutOfRangeException()
        };

        public static string KZ(SocketMessageComponent component) => component.Data.Values.ElementAt(0) switch
        {
            "timer-easy" => new CSServers().GetKZ().TIMER_EASY.GetRandom().Info(),
            "go-easy" => new CSServers().GetKZ().GO_EASY.GetRandom().Info(),
            "timer-middle" => new CSServers().GetKZ().TIMER_MEDIUM.GetRandom().Info(),
            "go-middle" => new CSServers().GetKZ().GO_MEDIUM.GetRandom().Info(),
            "timer-hard" => new CSServers().GetKZ().TIMER_HARD.GetRandom().Info(),
            "go-hard" => new CSServers().GetKZ().GO_HARD.GetRandom().Info(),
            _ => throw new IndexOutOfRangeException()
        };

        public static string PUBLIC(SocketMessageComponent component) => component.Data.Values.ElementAt(0) switch
        {
            "only-dust" => new CSServers().GetPUBLIC().ONLY_DUST2.GetRandom().Info(),
            "only-mirage" => new CSServers().GetPUBLIC().ONLY_MIRAGE.GetRandom().Info(),
            "no-limit" => new CSServers().GetPUBLIC().NO_LIMIT.GetRandom().Info(),
            "competitive" => new CSServers().GetPUBLIC().COMPETITIVE_MAPS.GetRandom().Info(),
            "wh" => new CSServers().GetPUBLIC().WH_ON.GetRandom().Info(),
            "all-maps" => new CSServers().GetPUBLIC().ALL_MAPS.GetRandom().Info(),
            "destr-inferno" => new CSServers().GetPUBLIC().DESTRUCTIBLE_INFERNO.GetRandom().Info(),
            _ => throw new IndexOutOfRangeException()
        };

        public static string AWP(SocketMessageComponent component) => component.Data.Values.ElementAt(0) switch
        {
            "cannons" => new CSServers().GetAWP().AWP_CANNONS.GetRandom().Info(),
            "lego" => new CSServers().GetAWP().ONLY_AWP_LEGO_2.GetRandom().Info(),
            "servers" => new CSServers().GetAWP().AWP_SERVERS.GetRandom().Info(),
            _ =>throw new IndexOutOfRangeException()
        };

        public static string HNS(SocketMessageComponent component) => component.Data.Values.ElementAt(0) switch
        {
            "servers" => new CSServers().GetHNS().HNS_SERVERS.GetRandom().Info(),
            "no-rules" => new CSServers().GetHNS().HNS_NO_RULES.GetRandom().Info(),
            "training" => new CSServers().GetHNS().HNS_TRAINING.GetRandom().Info(),
            _ => throw new IndexOutOfRangeException()
        };
    }
}
