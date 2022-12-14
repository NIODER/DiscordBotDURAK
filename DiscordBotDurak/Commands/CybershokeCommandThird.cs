using CyberShoke;
using CyberShoke.Objects;
using Discord;
using System;

namespace DiscordBotDurak.Commands
{
    public class CybershokeCommandThird : ICommand
    {
        private readonly string customId;
        private readonly string value;

        public CybershokeCommandThird(string customId, string value)
        {
            this.customId = customId;
            this.value = value;
        }

        public CommandResult GetResult()
        {
            var server = customId switch
            {
                "DUELS2X2" => Duplets(),
                "DUELS" => Duels(),
                "RETAKE" => Retake(),
                "RETAKECLASSIC" => RetakeClasiic(),
                "DM" => DM(),
                "PISTOLDM" => PISTOLDM(),
                "AWPDM" => AWPDM(),
                "AIMDM" => AIMDM(),
                "SURF" => SURF(),
                "BHOP" => BHOP(),
                "KZ" => KZ(),
                "PUBLIC" => PUBLIC(),
                "AWP" => AWP(),
                "HNS" => HNS(),
                _ => null
            };
            var exception = server is null ? new NotImplementedException("Not implemented id") : null;
            CommandResult commandResult;
            if (exception is null)
            {
                commandResult = new CommandResult().WithEmbed(new EmbedBuilder()
                    .WithTitle("Server")
                    .WithColor(Color.Blue)
                    .AddField("ip:", $"connect {server.ip}:{server.port}")
                    .AddField("fast connect:", $"steam://connect/{server.ip}:{server.port}")
                    .WithFooter("Cybershoke command")
                    .WithCurrentTimestamp());
            }
            else
            {
                commandResult = new CommandResult().WithException(exception);
                _ = Logger.Instance().LogAsync(
                    new LogMessage(
                        LogSeverity.Error,
                        GetType().Name,
                        $"Component custom id was not implemented. Id: {customId}",
                        exception));
            }
            return commandResult;
        }


        public Server Duplets() => value switch
        {
            "only-mirage" => new CSServers().GetDUELS2X2().ONLY_MIRAGE.GetRandom(),
            "only-dust" => new CSServers().GetDUELS2X2().ONLY_DUST2.GetRandom(),
            "all-maps" => new CSServers().GetDUELS2X2().ALL_MAPS.GetRandom(),
            _ => throw new IndexOutOfRangeException()
        };

        public Server Duels() => value switch
        {
            "only-mirage" => new CSServers().GetDUELS().ONLY_MIRAGE.GetRandom(),
            "only-dust" => new CSServers().GetDUELS().ONLY_DUST2.GetRandom(),
            "all-maps" => new CSServers().GetDUELS().ALL_MAPS.GetRandom(),
            _ => throw new IndexOutOfRangeException()
        };

        public Server Retake() => value switch
        {
            "easy" => new CSServers().GetRETAKE().EASY.GetRandom(),
            "hard" => new CSServers().GetRETAKE().HARD.GetRandom(),
            "9slots" => new CSServers().GetRETAKE().SLOTS9.GetRandom(),
            "7slots" => new CSServers().GetRETAKE().SLOTS7.GetRandom(),
            _ => throw new IndexOutOfRangeException()
        };

        public Server RetakeClasiic() => value switch
        {
            "easy" => new CSServers().GetRETAKECLASSIC().EASY.GetRandom(),
            "middle" => new CSServers().GetRETAKECLASSIC().MEDIUM.GetRandom(),
            "hard" => new CSServers().GetRETAKECLASSIC().HARD.GetRandom(),
            "9slots" => new CSServers().GetRETAKECLASSIC().SLOTS9.GetRandom(),
            "7slots" => new CSServers().GetRETAKECLASSIC().SLOTS7.GetRandom(),
            _ => throw new IndexOutOfRangeException()
        };

        public Server DM() => value switch
        {
            "easy" => new CSServers().GetDM().Easy.GetRandom(),
            "medium" => new CSServers().GetDM().Medium.GetRandom(),
            "hard" => new CSServers().GetDM().Hard.GetRandom(),
            _ => throw new IndexOutOfRangeException()
        };

        public Server PISTOLDM() => value switch
        {
            "hsdm" => new CSServers().GetPISTOLDM().HSDM.GetRandom(),
            "medium" => new CSServers().GetPISTOLDM().MEDIUM.GetRandom(),
            _ => throw new IndexOutOfRangeException()
        };

        public Server AWPDM() => value switch
        {
            "awpdm" => new CSServers().GetAWPDM().GetRandom(),
            _ => throw new IndexOutOfRangeException()
        };

        public Server AIMDM() => value switch
        {
            "classic" => new CSServers().GetAIM_DM().AIMDM.GetRandom(),
            "pistol" => new CSServers().GetAIM_DM().PISTOL_AIMDM.GetRandom(),
            _ => throw new IndexOutOfRangeException()
        };

        public Server SURF() => value switch
        {
            "beginner" => new CSServers().GetSURF().BEGINNER.GetRandom(),
            "easy" => new CSServers().GetSURF().EASY.GetRandom(),
            "normal" => new CSServers().GetSURF().NORMAL.GetRandom(),
            "medium" => new CSServers().GetSURF().MEDIUM.GetRandom(),
            "hard" => new CSServers().GetSURF().HARD.GetRandom(),
            "top" => new CSServers().GetSURF().TOP.GetRandom(),
            _ => throw new IndexOutOfRangeException()
        };

        public Server BHOP() => value switch
        {
            "easy" => new CSServers().GetBHOP().EASY.GetRandom(),
            "medium" => new CSServers().GetBHOP().MEDIUM.GetRandom(),
            "hard" => new CSServers().GetBHOP().HARD.GetRandom(),
            "legendary" => new CSServers().GetBHOP().LEGENDARY.GetRandom(),
            _ => throw new IndexOutOfRangeException()
        };

        public Server KZ() => value switch
        {
            "go-easy" => new CSServers().GetKZ().GO_EASY.GetRandom(),
            "go-middle" => new CSServers().GetKZ().GO_MEDIUM.GetRandom(),
            "go-hard" => new CSServers().GetKZ().GO_HARD.GetRandom(),
            _ => throw new IndexOutOfRangeException()
        };

        public Server PUBLIC() => value switch
        {
            "only-dust" => new CSServers().GetPUBLIC().ONLY_DUST2.GetRandom(),
            "only-mirage" => new CSServers().GetPUBLIC().ONLY_MIRAGE.GetRandom(),
            "trending" => new CSServers().GetPUBLIC().TRENDING.GetRandom(),
            "wh" => new CSServers().GetPUBLIC().WH_ON.GetRandom(),
            "all-maps" => new CSServers().GetPUBLIC().ALL_MAPS.GetRandom(),
            _ => throw new IndexOutOfRangeException()
        };

        public Server AWP() => value switch
        {
            "cannons" => new CSServers().GetAWP().AWP_CANNONS.GetRandom(),
            "lego" => new CSServers().GetAWP().ONLY_AWP_LEGO_2.GetRandom(),
            "servers" => new CSServers().GetAWP().AWP_SERVERS.GetRandom(),
            _ => throw new IndexOutOfRangeException()
        };

        public Server HNS() => value switch
        {
            "servers" => new CSServers().GetHNS().HNS_SERVERS.GetRandom(),
            "no-rules" => new CSServers().GetHNS().HNS_NO_RULES.GetRandom(),
            "training" => new CSServers().GetHNS().HNS_TRAINING.GetRandom(),
            _ => throw new IndexOutOfRangeException()
        };

        public Server MULTICFGDM() => value switch
        {
            "multicfgm" => new CSServers().GetMULTICFGDM().GetRandom(),
            _ => throw new IndexOutOfRangeException()
        };
    }
}
