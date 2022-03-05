﻿using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.IO;
using System.Collections.Generic;
using EthernetFunctons.Balaboba;
using Constants;
using DiscordBotDURAK.EthernetFunctions;
using System.Threading;
using CyberShoke;
using CyberShoke.Objects;
using Discord.Commands;
using System.Linq;

namespace DiscordBotDURAK
{
    class Program
    {
        DiscordSocketClient client;
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            client = new DiscordSocketClient();
            client.MessageReceived += CommandsHandler;
            client.SelectMenuExecuted += SelectMenuHandler;
            client.Log += Log;

            string token = null;

            using (StreamReader reader = new StreamReader(File.OpenRead(Constants.Constants.tokenPath)))
            {
                token = reader.ReadToEnd();
            }

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            await client.SetGameAsync("$help");

            Console.ReadLine();
        }

        public static Task Log(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }

        public async Task SelectMenuHandler(SocketMessageComponent component)
        {
            switch (component.Data.CustomId)
            {
                case "cs-main":
                    SelectMenuBuilder selectMenuBuilder = new SelectMenuBuilder()
                        .WithMinValues(1)
                        .WithMaxValues(1);
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
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetMULTICFGDM().GetRandom().Info());
                            return;
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
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetEXECUTE().GetRandom().Info());
                            return;
                        case CybershokeCategories.PISTOLRETAKE:
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetPISTOLRETAKE().GetRandom().Info());
                            return;
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
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetARENA().GetRandom().Info());
                            return;
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
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetMANIAC().GetRandom().Info());
                            return;
                        case CybershokeCategories.PROPHUNT:
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetPROPHUNT().GetRandom().Info());
                            return;
                        case CybershokeCategories.HNS:
                            selectMenuBuilder.WithCustomId("HNS")
                                .AddOption("HNS SERVERS", "servers")
                                .AddOption("HNS NO RULES", "no-rules")
                                .AddOption("HNS TRAINING", "training");
                            break;
                        case CybershokeCategories.KNIFE:
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetKNIFE().GetRandom().Info());
                            return;
                        default:
                            break;
                    }
                    ComponentBuilder componentBuilder = new ComponentBuilder().WithSelectMenu(selectMenuBuilder);
                    await component.Channel.SendMessageAsync("Режимы:", components: componentBuilder.Build());
                    break;

                case "DUELS2X2":
                    switch (component.Data.Values.ElementAt(0))
                    {
                        case "only-mirage":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetDUELS2X2().ONLY_MIRAGE.GetRandom().Info());
                            break;
                        case "only-dust":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetDUELS2X2().ONLY_DUST2.GetRandom().Info());
                            break;
                        case "all-maps":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetDUELS2X2().ALL_MAPS.GetRandom().Info());
                            break;
                        default:
                            break;
                    }
                    break;
                case "DUELS":
                    switch (component.Data.Values.ElementAt(0))
                    {
                        case "only-mirage":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetDUELS().ONLY_MIRAGE.GetRandom().Info());
                            break;
                        case "only-dust":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetDUELS().ONLY_DUST2.GetRandom().Info());
                            break;
                        case "all-maps":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetDUELS().ALL_MAPS.GetRandom().Info());
                            break;
                        default:
                            break;
                    }
                    break;
                case "RETAKE":
                    switch (component.Data.Values.ElementAt(0))
                    {
                        case "easy":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetRETAKE().EASY.GetRandom().Info());
                            break;
                        case "hard":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetRETAKE().HARD.GetRandom().Info());
                            break;
                        case "9slots":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetRETAKE().SLOTS9.GetRandom().Info());
                            break;
                        case "7slots":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetRETAKE().SLOTS7.GetRandom().Info());
                            break;
                        default:
                            break;
                    }
                    break;
                case "RETAKECLASSIC":
                    switch (component.Data.Values.ElementAt(0))
                    {
                        case "easy":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetRETAKECLASSIC().EASY.GetRandom().Info());
                            break;
                        case "middle":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetRETAKECLASSIC().MEDIUM.GetRandom().Info());
                            break;
                        case "hard":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetRETAKECLASSIC().HARD.GetRandom().Info());
                            break;
                        case "9slots":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetRETAKECLASSIC().SLOTS9.GetRandom().Info());
                            break;
                        case "7slots":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetRETAKECLASSIC().SLOTS7.GetRandom().Info());
                            break;
                        default:
                            break;
                    }
                    break;
                case "DM":
                    switch (component.Data.Values.ElementAt(0))
                    {
                        case "18easy":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetDM().EASY18.GetRandom().Info());
                            break;
                        case "16easy":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetDM().EASY16.GetRandom().Info());
                            break;
                        case "14easy":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetDM().EASY14.GetRandom().Info());
                            break;
                        case "20lite":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetDM().LITE20.GetRandom().Info());
                            break;
                        case "18lite":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetDM().LITE18.GetRandom().Info());
                            break;
                        case "16lite":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetDM().LITE16.GetRandom().Info());
                            break;
                        case "18slots":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetDM().SLOTS18.GetRandom().Info());
                            break;
                        case "16slots":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetDM().SLOTS16.GetRandom().Info());
                            break;
                        case "noawp":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetDM().NOAWP.GetRandom().Info());
                            break;
                        default:
                            break;
                    }
                    break;
                case "HSDM":
                    switch (component.Data.Values.ElementAt(0))
                    {
                        case "lite":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetHSDM().HSDM_LITE.GetRandom().Info());
                            break;
                        case "classic":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetHSDM().HSDM.GetRandom().Info());
                            break;
                        case "onetap":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetHSDM().HSDM_ONETAP.GetRandom().Info());
                            break;
                        default:
                            break;
                    }
                    break;
                case "PISTOLDM":
                    switch (component.Data.Values.ElementAt(0))
                    {
                        case "hsdm":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetPISTOLDM().PISTOL_HSDM.GetRandom().Info());
                            break;
                        case "lite":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetPISTOLDM().PISTOLDM_LITE.GetRandom().Info());
                            break;
                        case "classic":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetPISTOLDM().PISTOLDM.GetRandom().Info());
                            break;
                        default:
                            break;
                    }
                    break;
                case "AWPDM":
                    switch (component.Data.Values.ElementAt(0))
                    {
                        case "lite":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetAWPDM().AWPDM_LITE.GetRandom().Info());
                            break;
                        case "classic":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetAWPDM().AWPDM.GetRandom().Info());
                            break;
                        case "noscope":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetAWPDM().NOSCOPEDM.GetRandom().Info());
                            break;
                        default:
                            break;
                    }
                    break;
                case "AIMDM":
                    switch (component.Data.Values.ElementAt(0))
                    {
                        case "classic":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetAIM_DM().AIMDM.GetRandom().Info());
                            break;
                        case "pistol":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetAIM_DM().PISTOL_AIMDM.GetRandom().Info());
                            break;
                        default:
                            break;
                    }
                    break;
                case "SURF":
                    switch (component.Data.Values.ElementAt(0))
                    {
                        case "beginner":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetSURF().BEGINNER.GetRandom().Info());
                            break;
                        case "easy":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetSURF().EASY.GetRandom().Info());
                            break;
                        case "normal":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetSURF().NORMAL.GetRandom().Info());
                            break;
                        case "medium":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetSURF().MEDIUM.GetRandom().Info());
                            break;
                        case "hard":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetSURF().HARD.GetRandom().Info());
                            break;
                        case "top":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetSURF().TOP.GetRandom().Info());
                            break;
                        default:
                            break;
                    }
                    break;
                case "BHOP":
                    switch (component.Data.Values.ElementAt(0))
                    {
                        case "easy":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetBHOP().EASY.GetRandom().Info());
                            break;
                        case "medium":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetBHOP().MEDIUM.GetRandom().Info());
                            break;
                        case "hard":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetBHOP().HARD.GetRandom().Info());
                            break;
                        case "legendary":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetBHOP().LEGEMDARY.GetRandom().Info());
                            break;
                        case "tick":
                            await component.Channel.SendMessageAsync(
                                new CSServers().GetBHOP().TICK.GetRandom().Info());
                            break;
                        default:
                            break;
                    }
                    break;
                case "KZ":
                    switch (component.Data.Values.ElementAt(0))
                    {
                        case "timer-easy":
                            await component.Channel.SendMessageAsync(new CSServers().GetKZ().TIMER_EASY.GetRandom().Info());
                            break;
                        case "go-easy":
                            await component.Channel.SendMessageAsync(new CSServers().GetKZ().GO_EASY.GetRandom().Info());
                            break;
                        case "timer-middle":
                            await component.Channel.SendMessageAsync(new CSServers().GetKZ().TIMER_MEDIUM.GetRandom().Info());
                            break;
                        case "go-middle":
                            await component.Channel.SendMessageAsync(new CSServers().GetKZ().GO_MEDIUM.GetRandom().Info());
                            break;
                        case "timer-hard":
                            await component.Channel.SendMessageAsync(new CSServers().GetKZ().TIMER_HARD.GetRandom().Info());
                            break;
                        case "go-hard":
                            await component.Channel.SendMessageAsync(new CSServers().GetKZ().GO_HARD.GetRandom().Info());
                            break;
                        default:
                            break;
                    }
                    break;
                case "PUBLIC":
                    switch (component.Data.Values.ElementAt(0))
                    {
                        case "only-dust":
                            await component.Channel.SendMessageAsync(new CSServers().GetPUBLIC().ONLY_DUST2.GetRandom().Info());
                            break;
                        case "only-mirage":
                            await component.Channel.SendMessageAsync(new CSServers().GetPUBLIC().ONLY_MIRAGE.GetRandom().Info());
                            break;
                        case "no-limit":
                            await component.Channel.SendMessageAsync(new CSServers().GetPUBLIC().NO_LIMIT.GetRandom().Info());
                            break;
                        case "competitive":
                            await component.Channel.SendMessageAsync(new CSServers().GetPUBLIC().COMPETITIVE_MAPS.GetRandom().Info());
                            break;
                        case "wh":
                            await component.Channel.SendMessageAsync(new CSServers().GetPUBLIC().WH_ON.GetRandom().Info());
                            break;
                        case "all-maps":
                            await component.Channel.SendMessageAsync(new CSServers().GetPUBLIC().ALL_MAPS.GetRandom().Info());
                            break;
                        case "destr-inferno":
                            await component.Channel.SendMessageAsync(new CSServers().GetPUBLIC().DESTRUCTIBLE_INFERNO.GetRandom().Info());
                            break;
                        default:
                            break;
                    }
                    break;
                case "AWP":
                    switch (component.Data.Values.ElementAt(0))
                    {
                        case "cannons":
                            await component.Channel.SendMessageAsync(new CSServers().GetAWP().AWP_CANNONS.GetRandom().Info());
                            break;
                        case "lego":
                            await component.Channel.SendMessageAsync(new CSServers().GetAWP().ONLY_AWP_LEGO_2.GetRandom().Info());
                            break;
                        case "servers":
                            await component.Channel.SendMessageAsync(new CSServers().GetAWP().AWP_SERVERS.GetRandom().Info());
                            break;
                        default:
                            break;
                    }
                    break;
                case "HNS":
                    switch (component.Data.Values.ElementAt(0))
                    {
                        case "servers":
                            await component.Channel.SendMessageAsync(new CSServers().GetHNS().HNS_SERVERS.GetRandom().Info());
                            break;
                        case "no-rules":
                            await component.Channel.SendMessageAsync(new CSServers().GetHNS().HNS_NO_RULES.GetRandom().Info());
                            break;
                        case "training":
                            await component.Channel.SendMessageAsync(new CSServers().GetHNS().HNS_TRAINING.GetRandom().Info());
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            await component.DeferAsync();
            await DeleteMessageAsync(component.Message, true, 60000); ;
        }

        private async Task CommandsHandler(SocketMessage message)
        {
            CheckGuilds(client.Guilds);
            if (!message.Channel.isDirect() && message.Channel.ChannelType() == ChannelSeverity.NoSuchChannel)
            {
                MyDatabase.AddChannel(message.GuildId(), message.Channel.Id);
                await message.Channel.SendMessageAsync("Этот канал помечен типом \"флуд\", чтобы " +
                    "показать сводку типов, а также узнать, как изменить тип " +
                    "напишите $help");
            }
            if (CheckChannel(message))
            {
                return;
            }
            if ((message.MentionedUsers.Count != 0 || message.MentionedRoles.Count != 0) && !message.Channel.IsReferences())
            {
                await DeleteMessageAsync(message, enableTimer: true);
            }
            if (message.Author.IsBot)
            {
                return;
            }

            if (message.Content.StartsWith(Commands.radio))
            {
                GetRadio(message);
            }
            if (message.Content.StartsWith(Commands.joke))
            {
                GetJoke(message);
            }
            if (message.Content.ToLower().StartsWith(Commands.quote))
            {
                MortarQuote(message);
            }
            if (message.Content.StartsWith(Commands.search))
            {
                SearchMessages(message);
            }
            if (message.Content.StartsWith(Commands.help))
            {
                CommandsHelp(message);
            }
            if (message.Content.ToLower().StartsWith(Commands.random))
            {
                RAND_Func(message);
            }
            if (message.Content.ToLower().Contains(Commands.decide))
            {
                Desider(message);
            }
            if (message.Content == Commands.id)
            {
                SendId(message);
            }
            if (RandomMessages.TriggerCheck(message.Content.ToLower()))
            {
                SHITPOST_Func(message);
            }
            if (message.Content.Contains("http"))
            {
                RefModeration(message);
            }
            if (message.Content.StartsWith(Commands.cyberShoke))
            {
                GetCSGOServer(message);
            }
            if (message.IsAuthorAdmin())
            {
                if (message.Content.StartsWith(Commands.deleteFavorRadio))
                {
                    DeleteFavour(message);
                }
                if (message.Content.StartsWith(Commands.setRadio))
                {
                    SetFavour(message);
                }
                if (message.Content.StartsWith(Commands.joinedAt))
                {
                    WhenJoined(message);
                }
                if (message.Content.ToLower() == Commands.owner)
                {
                    OwnerHelp(message);
                }
                if (message.Content.StartsWith(Commands.setChannelType))
                {
                    SetChannelType(message);
                }
                if (message.Content.ToLower().StartsWith(Commands.spam))
                {
                    SPAM_Func(message);
                }
                if (message.Content.ToLower().StartsWith(Commands.delete))
                {
                    DeleteFunc(message);
                }
                if (message.Content.ToLower().StartsWith(Commands.moderate))
                {
                    Moderate(message);
                }
                if (message.Content.ToLower().Contains(Commands.clean))
                {
                    Clear(message, Commands.clean);
                }
                if (message.Content.StartsWith(Commands.giveAdmin))
                {
                    GiveAdmin(message);
                }
            }
        }

        #region internal

        private void CheckGuilds(IReadOnlyCollection<SocketGuild> guilds)
        {
            var dbguilds = MyDatabase.GetGuilds();
            bool delete;
            foreach (var dbguild in dbguilds)
            {
                delete = true;
                foreach (var guild in guilds)
                {
                    if (dbguild == guild.Id)
                    {
                        delete = false;
                        break;
                    }
                }
                if (delete)
                {
                    MyDatabase.DeleteGuild(Convert.ToString(dbguild));
                }
            }
            bool add;
            dbguilds = MyDatabase.GetGuilds();
            foreach (var guild in guilds)
            {
                add = true;

                foreach (var dbguild in dbguilds)
                {
                    if (dbguild == guild.Id)
                    {
                        add = false;
                        break;
                    }
                }
                if (add)
                {
                    AddGuildInDB(guild);
                }
            }
        }

        /// <summary>
        /// Check if bot can moderate this channel
        /// </summary>
        /// <param name="message">message</param>
        /// <returns>true if bot can moderate this channel</returns>
        private bool CheckChannel(SocketMessage message)
        {
            if (message.Channel.GetType().ToString() == "Discord.WebSocket.SocketDMChannel")
            {
                return true;
            }
            if (!((SocketGuildChannel)message.Channel).Guild.Check())
            {
                AddGuildInDB(message);
            }
            return message.Channel.ChannelType() != ChannelSeverity.None;
        }

        private void AddGuildInDB(SocketMessage message)
        {
            SocketGuild guild = ((SocketGuildChannel)message.Channel).Guild;
            string guildId = message.GuildId();
            MyDatabase.AddGuild(guildId);
            MyDatabase.AddAdmin(guildId, guild.OwnerId);
            foreach (var channel in guild.Channels)
            {
                MyDatabase.AddChannel(guildId, channel.Id);
            }
            Hello(guild.Owner);
        }

        private void AddGuildInDB(SocketGuild guild)
        {
            string guildId = Convert.ToString(guild.Id);
            MyDatabase.AddGuild(guildId);
            MyDatabase.AddAdmin(guildId, guild.OwnerId);
            foreach (var channel in guild.Channels)
            {
                MyDatabase.AddChannel(guildId, channel.Id);
            }
            Hello(guild.Owner);
        }



        private async void Hello(SocketGuildUser owner)
        {
            await owner.SendMessageAsync($"Привет, я бот-дурак, ты владелец сервера, давай дружить!");
            await owner.SendMessageAsync("Сейчас ты - единственный и неповторимы админ для меня на этом сервере) " +
                "нужно будет добавить еще админов(если хочешь) и обязательно настроить чаты! Когда захочешь это сделать " +
                "напиши мне команду $owner (будет много букв, но я верю в тебя)");
        }

        /// <summary>
        /// Sets a channel type
        /// </summary>
        /// <param name="message">message</param>
        private void SetChannelType(SocketMessage message)
        {
            int severity = 0;
            string msg = message.Content.Substring(Commands.setChannelType.Length).Trim().TrimEnd();
            try
            {
                severity = Convert.ToInt32(msg.Split(' ')[1]);
            }
            catch (IndexOutOfRangeException)
            {
                message.Channel.SendMessageAsync("Неправильная команда");
                return;
            }
            
            string mention = msg.Split(' ')[0];
            ulong id;
            if (mention.StartsWith("<#") && mention.EndsWith(">"))
            {
                id = Convert.ToUInt64(mention.Remove(0,2).Remove(18));
            }
            else
            {
                message.Channel.SendMessageAsync("Неправильно упомянут канал, канал упоминается в формате # + название канала");
                return;
            }
            if (Enum.IsDefined(typeof(ChannelSeverity), severity))
            {
                MyDatabase.UpdateChannelType(message.GuildId(), id, (ChannelSeverity)severity);
            }
            else
            {
                message.Channel.SendMessageAsync("Неправильно указан тип канала");
            }
        }
        
        /// <summary>
        /// Deletes message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="enableTimer">true enables timer</param>
        /// <param name="timer">value of timer </param>
        /// <returns></returns>
        private async Task DeleteMessageAsync(SocketMessage message, bool enableTimer, int timer = 30000)
        {
            if (enableTimer)
            {
                if (message.Channel.ChannelType() != ChannelSeverity.None || message.Channel.IsReferences())
                {
                    await Task.Delay(timer);
                    await message.Channel.DeleteMessageAsync(message.Id);
                }
            }
            else
            {
                await message.Channel.DeleteMessageAsync(message.Id);
            }
            await Log(new(LogSeverity.Info, Sources.internal_function, "Message deleted"));
        }

        #endregion

        #region functions
        private async void GetCSGOServer(SocketMessage message)
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

            await message.Channel.SendMessageAsync(text: "Доступно:", components: builder.Build());
        }

        #region CS:GO
        private string GetAimDM(string[] msg)
        {
            CSServers cybershoke = new CSServers();
            if (msg.Length < 3)
            {
                return "1 - Aim DM\n2 - Pistol Aim DM";
            }
            switch (msg[2])
            {
                case "1":
                    return cybershoke.GetAIM_DM().AIMDM.GetRandom().Info();
                case "2":
                    return cybershoke.GetAIM_DM().PISTOL_AIMDM.GetRandom().Info();
                default:
                    return "1 - Aim DM\n2 - Pistol Aim DM";
            }
        }

        private string GetAmongUs() => new CSServers().GetAMONGUS().GetRandom().Info();

        private string GetArena() => new CSServers().GetARENA().GetRandom().Info();

        private string GetAwp(string[] msg)
        {
            CSServers cybershoke = new();
            if (msg.Length < 3)
            {
                return "1 - AWP CANNONS\n2 - ONLY AWP LEGO 2\n3 - AWP SERVERS";
            }
            switch (msg[2])
            {
                case "1":
                    return cybershoke.GetAWP().AWP_CANNONS.GetRandom().Info();
                case "2":
                    return cybershoke.GetAWP().ONLY_AWP_LEGO_2.GetRandom().Info();
                case "3":
                    return cybershoke.GetAWP().AWP_SERVERS.GetRandom().Info();
                default:
                    return "1 - AWP CANNONS\n2 - ONLY AWP LEGO 2\n3 - AWP SERVERS";
            }
        }

        private string GetAwpDM(string[] msg)
        {
            CSServers cyberShoke = new();
            if (msg.Length < 3)
            {
                return "1 - AWPDM LITE\n2 - AWPDM\n3 - NOSCOPEDM";
            }
            switch (msg[2])
            {
                case "1":
                    return cyberShoke.GetAWPDM().AWPDM_LITE.GetRandom().Info();
                case "2":
                    return cyberShoke.GetAWPDM().AWPDM.GetRandom().Info();
                case "3":
                    return cyberShoke.GetAWPDM().NOSCOPEDM.GetRandom().Info();
                default:
                    return "1 - AWPDM LITE\n2 - AWPDM\n3 - NOSCOPEDM";
            }
        }

        private string GetBhop(string[] msg)
        {
            CSServers cybershoke = new();
            if (msg.Length < 3)
            {
                return "1 - 64 tick\n2 - easy\n3 - medium\n4 - hard\n5 - legendary";
            }
            switch (msg[2])
            {
                case "1":
                    return cybershoke.GetBHOP().TICK.GetRandom().Info();
                case "2":
                    return cybershoke.GetBHOP().EASY.GetRandom().Info();
                case "3":
                    return cybershoke.GetBHOP().MEDIUM.GetRandom().Info();
                case "4":
                    return cybershoke.GetBHOP().HARD.GetRandom().Info();
                case "5":
                    return cybershoke.GetBHOP().LEGEMDARY.GetRandom().Info();
                default:
                    return "1 - 64 tick\n2 - easy\n3 - medium\n4 - hard\n5 - legendary";
            }
        }

        private string GetDeathrun(string[] msg)
        {
            CSServers cybershoke = new();
            if (msg.Length < 3)
            {
                return "1 - EASY\n2 - WARMUP";
            }
            switch (msg[2])
            {
                case "1":
                    return cybershoke.GetDEATHRUN().EASY.GetRandom().Info();
                case "2":
                    return cybershoke.GetDEATHRUN().WARMUP.GetRandom().Info();
                default:
                    return "1 - EASY\n2 - WARMUP";
            }
        }

        private string GetDM(string[] msg)
        {
            CSServers cyberShoke = new();
            if (msg.Length < 3)
            {
                return "1 - 18 SLOTS LITE 1-3LVL FACEIT\n2 - 16 SLOTS LITE 1-3LVL FACEIT\n" +
                    "3 - 14 SLOTS LITE 1-3LVL FACEIT\n4 - 20 SLOTS LITE\n5 - 18 SLOTS LITE\n6 - 18 SLOTS\n" +
                    "7 - 16 SLOTS LITE\n8 - 16 SLOTS\n9 - NOAWP";
            }
            switch (msg[2])
            {
                case "1":
                    return cyberShoke.GetDM().EASY18.GetRandom().Info();
                case "2":
                    return cyberShoke.GetDM().EASY16.GetRandom().Info();
                case "3":
                    return cyberShoke.GetDM().EASY14.GetRandom().Info();
                case "4":
                    return cyberShoke.GetDM().LITE20.GetRandom().Info();
                case "5":
                    return cyberShoke.GetDM().LITE18.GetRandom().Info();
                case "6":
                    return cyberShoke.GetDM().SLOTS18.GetRandom().Info();
                case "7":
                    return cyberShoke.GetDM().LITE16.GetRandom().Info();
                case "8":
                    return cyberShoke.GetDM().SLOTS16.GetRandom().Info();
                case "9":
                    return cyberShoke.GetDM().NOAWP.GetRandom().Info();
                default:
                    return "1 - 18 SLOTS LITE 1-3LVL FACEIT\n2 - 16 SLOTS LITE 1-3LVL FACEIT\n" +
                        "3 - 14 SLOTS LITE 1-3LVL FACEIT\n4 - 20 SLOTS LITE\n5 - 18 SLOTS LITE\n6 - 18 SLOTS\n" +
                        "7 - 16 SLOTS LITE\n8 - 16 SLOTS\n9 - NOAWP";
            }
        }

        private string GetDuels(string[] msg)
        {
            CSServers cyberShoke = new();
            if (msg.Length < 3)
            {
                return "1 - ONLY MIRAGE\n2 - ONLY DUST2\n3 - ALL MAPS";
            }
            switch (msg[2])
            {
                case "1":
                    return cyberShoke.GetDUELS().ONLY_MIRAGE.GetRandom().Info();
                case "2":
                    return cyberShoke.GetDUELS().ONLY_DUST2.GetRandom().Info();
                case "3":
                    return cyberShoke.GetDUELS().ALL_MAPS.GetRandom().Info();
                default:
                    return "1 - ONLY MIRAGE\n2 - ONLY DUST2\n3 - ALL MAPS";
            }
        }

        private string GetDuplets(string[] msg)
        {
            CSServers cyberShoke = new();
            if (msg.Length < 3)
            {
                return "1 - ONLY MIRAGE\n2 - ONLY DUST2\n3 - ALL MAPS";
            }
            switch (msg[2])
            {
                case "1":
                    return cyberShoke.GetDUELS2X2().ONLY_MIRAGE.GetRandom().Info();
                case "2":
                    return cyberShoke.GetDUELS2X2().ONLY_DUST2.GetRandom().Info();
                case "3":
                    return cyberShoke.GetDUELS2X2().ALL_MAPS.GetRandom().Info();
                default:
                    return "1 - ONLY MIRAGE\n2 - ONLY DUST2\n3 - ALL MAPS";
            }
        }

        private string GetExecute() => new CSServers().GetEXECUTE().GetRandom().Info();

        private string GetHNS(string[] msg)
        {
            CSServers cyberShoke = new();
            if (msg.Length < 3)
            {
                return "1 - HNS SERVERS\n2 - HNS NO RULES\n3 - HNS TRAINING";
            }
            IEnumerable<Server> list;
            switch (msg[2])
            {
                case "1":
                    list = cyberShoke.GetHNS().HNS_SERVERS;
                    break;
                case "2":
                    list = cyberShoke.GetHNS().HNS_NO_RULES;
                    break;
                case "3":
                    list = cyberShoke.GetHNS().HNS_TRAINING;
                    break;
                default:
                    return "1 - HNS SERVERS\n2 - HNS NO RULES\n3 - HNS TRAINING";
            }
            return list.GetRandom().Info();
        }

        private string GetHSDM(string[] msg)
        {
            CSServers servers = new();
            if (msg.Length < 3)
            {
                return "1 - HSDM LITE\n2 - HSDM\n3 - HSDM ONETAP";
            }
            IEnumerable<Server> list;
            switch (msg[2])
            {
                case "1":
                    list = servers.GetHSDM().HSDM_LITE;
                    break;
                case "2":
                    list = servers.GetHSDM().HSDM;
                    break;
                case "3":
                    list = servers.GetHSDM().HSDM_ONETAP;
                    break;
                default:
                    return "1 - HSDM LITE\n2 - HSDM\n3 - HSDM ONETAP";
            }
            return list.GetRandom().Info();
        }

        private string GetJail(string[] msg)
        {
            CSServers servers = new();
            if (msg.Length < 3)
            {
                return "1 - CT 16\n2 - CT 14\n3 - CT 0";
            }
            IEnumerable<Server> list;
            switch (msg[2])
            {
                case "1":
                    list = servers.GetJAIL().CT_16;
                    break;
                case "2":
                    list = servers.GetJAIL().CT_14;
                    break;
                case "3":
                    list = servers.GetJAIL().CT_0;
                    break;
                default:
                    return "1 - CT 16\n2 - CT 14\n3 - CT 0";
            }
            return list.GetRandom().Info();
        }

        private string GetKnife() => new CSServers().GetKNIFE().GetRandom().Info();

        private string GetKZ(string[] msg)
        {
            CSServers servers = new();
            if (msg.Length < 3)
            {
                return "1 - KZTimer - TIER 1-2\n" +
                    "GOKZ - TIER 1-2\n" +
                    "KZTimer - TIER 3-4\n" +
                    "GOKZ - TIER 3-4\n" +
                    "KZTimer - TIER 5-6\n" +
                    "GOKZ - TIER 5-6";
            }
            IEnumerable<Server> list;
            switch (msg[2])
            {
                case "1":
                    list = servers.GetKZ().TIMER_EASY;
                    break;
                case "2":
                    list = servers.GetKZ().GO_EASY;
                    break;
                case "3":
                    list = servers.GetKZ().TIMER_MEDIUM;
                    break;
                case "4":
                    list = servers.GetKZ().GO_MEDIUM;
                    break;
                case "5":
                    list = servers.GetKZ().TIMER_HARD;
                    break;
                case "6":
                    list = servers.GetKZ().GO_HARD;
                    break;
                default:
                    return "1 - KZTimer - TIER 1-2\n" +
                    "GOKZ - TIER 1-2\n" +
                    "KZTimer - TIER 3-4\n" +
                    "GOKZ - TIER 3-4\n" +
                    "KZTimer - TIER 5-6\n" +
                    "GOKZ - TIER 5-6";
            }
            return list.GetRandom().Info();
        }

        private string GetManiac() => new CSServers().GetMANIAC().GetRandom().Info();

        private string GetMinigames(string[] msg)
        {
            CSServers servers = new();
            if (msg.Length < 3)
            {
                return "1 - FUN MAPS\n2 - BATTLE MAPS";
            }
            IEnumerable<Server> list;
            switch (msg[2])
            {
                case "1":
                    list = servers.GetMINIGAMES().FUN_MAPS;
                    break;
                case "2":
                    list = servers.GetMINIGAMES().BATTLE_MAPS;
                    break;
                default:
                    return "1 - FUN MAPS\n2 - BATTLE MAPS";
            }
            return list.GetRandom().Info();
        }

        private string GetMulticFGDM() => new CSServers().GetMULTICFGDM().GetRandom().Info();

        private string GetPistolDM(string[] msg)
        {
            CSServers servers = new();
            if (msg.Length < 3)
            {
                return "1 - PISTOL HSDM\n2 - PISTOLDM LITE\n3 - PISTOLDM";
            }
            IEnumerable<Server> list;
            switch (msg[2])
            {
                case "1":
                    list = servers.GetPISTOLDM().PISTOL_HSDM;
                    break;
                case "2":
                    list = servers.GetPISTOLDM().PISTOLDM_LITE;
                    break;
                case "3":
                    list = servers.GetPISTOLDM().PISTOLDM;
                    break;
                default:
                    return "1 - PISTOL HSDM\n2 - PISTOLDM LITE\n3 - PISTOLDM";
            }
            return list.GetRandom().Info();
        }

        private string GetPistolRetake() => new CSServers().GetPISTOLRETAKE().GetRandom().Info();

        private string GetPropHunt()
        {
            return new CSServers().GetPROPHUNT().GetRandom().Info();
        }

        private string GetPublic(string[] msg)
        {
            CSServers servers = new();
            if (msg.Length < 3)
            {
                return "1 - ONLY DUST2\n" +
                    "2 - ONLY MIRAGE\n" +
                    "3 - NO LIMIT\n" +
                    "4 - COMPETITIVE MAPS\n" +
                    "5 - WH ON\n" +
                    "6 - ALL MAPS\n" +
                    "7 - DESTRUCTIBLE INFERNO";
            }
            IEnumerable<Server> list;
            switch (msg[2])
            {
                case "1":
                    list = servers.GetPUBLIC().ONLY_DUST2;
                    break;
                case "2":
                    list = servers.GetPUBLIC().ONLY_MIRAGE;
                    break;
                case "3":
                    list = servers.GetPUBLIC().NO_LIMIT;
                    break;
                case "4":
                    list = servers.GetPUBLIC().COMPETITIVE_MAPS;
                    break;
                case "5":
                    list = servers.GetPUBLIC().WH_ON;
                    break;
                case "6":
                    list = servers.GetPUBLIC().ALL_MAPS;
                    break;
                case "7":
                    list = servers.GetPUBLIC().DESTRUCTIBLE_INFERNO;
                    break;
                default:
                    return "1 - ONLY DUST2\n" +
                    "2 - ONLY MIRAGE\n" +
                    "3 - NO LIMIT\n" +
                    "4 - COMPETITIVE MAPS\n" +
                    "5 - WH ON\n" +
                    "6 - ALL MAPS\n" +
                    "7 - DESTRUCTIBLE INFERNO";
            }
            return list.GetRandom().Info();
        }

        private string GetRetake(string[] msg)
        {
            CSServers servers = new();
            if (msg.Length < 3)
            {
                return "1 - 1-3 LVL FACEIT\n" +
                    "2 - 8-10 LVL FACEIT\n" +
                    "3 - 9 SLOTS\n" +
                    "4 - 7 SLOTS";
            }
            IEnumerable<Server> list;
            switch (msg[2])
            {
                case "1":
                    list = servers.GetRETAKE().EASY;
                    break;
                case "2":
                    list = servers.GetRETAKE().HARD;
                    break;
                case "3":
                    list = servers.GetRETAKE().SLOTS9;
                    break;
                case "4":
                    list = servers.GetRETAKE().SLOTS7;
                    break;
                default:
                    return "1 - 1-3 LVL FACEIT\n" +
                    "2 - 8-10 LVL FACEIT\n" +
                    "3 - 9 SLOTS\n" +
                    "4 - 7 SLOTS";
            }
            return list.GetRandom().Info();
        }

        private string GetRetakeClassic(string[] msg)
        {
            CSServers servers = new();
            if (msg.Length < 3)
            {
                return "1 - 1-3 LVL FACEIT\n" +
                    "2 - 4-7 LVL FACEIT\n" +
                    "3 - 8-10 LVL FACEIT\n" +
                    "4 - OPEN TO ALL - 9 SLOTS\n" +
                    "5 - OPEN TO ALL - 7 SLOTS";
            }
            IEnumerable<Server> list;
            switch (msg[2])
            {
                case "1":
                    list = servers.GetRETAKECLASSIC().EASY;
                    break;
                case "2":
                    list = servers.GetRETAKECLASSIC().MEDIUM;
                    break;
                case "3":
                    list = servers.GetRETAKECLASSIC().HARD;
                    break;
                case "4":
                    list = servers.GetRETAKECLASSIC().SLOTS9;
                    break;
                case "5":
                    list = servers.GetRETAKECLASSIC().SLOTS7;
                    break;
                default:
                    return "1 - 1-3 LVL FACEIT\n" +
                    "2 - 8-10 LVL FACEIT\n" +
                    "3 - 4-7 LVL FACEIT\n" +
                    "4 - OPEN TO ALL - 9 SLOTS\n" +
                    "5 - OPEN TO ALL - 7 SLOTS";
            }
            return list.GetRandom().Info();
        }

        private string GetShokeLobby() => new CSServers().GetSHOKELOBBY().GetRandom().Info();

        private string GetSurf(string[] msg)
        {
            if (msg.Length < 3)
            {
                return "1 - TIER 1 - BEGINNER\n" +
                    "2 - TIER 1-2 - EASY\n" +
                    "3 - TIER 1-3 - NORMAL\n" +
                    "4 - TIER 3-4 - MEDIUM\n" +
                    "5 - TIER 3-5 - HARD\n" +
                    "6 - TIER 4-8 - TOP 350";
            }
            CSServers servers = new();
            switch (msg[2])
            {
                case "1":
                    return servers.GetSURF().BEGINNER.GetRandom().Info();
                case "2":
                    return servers.GetSURF().EASY.GetRandom().Info();
                case "3":
                    return servers.GetSURF().NORMAL.GetRandom().Info();
                case "4":
                    return servers.GetSURF().MEDIUM.GetRandom().Info();
                case "5":
                    return servers.GetSURF().HARD.GetRandom().Info();
                case "6":
                    return servers.GetSURF().TOP.GetRandom().Info();
                default:
                    return "1 - TIER 1 - BEGINNER\n" +
                    "2 - TIER 1-2 - EASY\n" +
                    "3 - TIER 1-3 - NORMAL\n" +
                    "4 - TIER 3-4 - MEDIUM\n" +
                    "5 - TIER 3-5 - HARD\n" +
                    "6 - TIER 4-8 - TOP 350";
            }
        }

        private string GetSurfCombat() => new CSServers().GetSURFCOMBAT().GetRandom().Info();

        private string GetZombieEscape() => new CSServers().GetZOMBIEESCAPE().GetRandom().Info();
        #endregion

        private async void DeleteFavour(SocketMessage message)
        {
            string reference = message.Content.Split(' ')[1];
            MyDatabase.DeleteRadio(message.GuildId(), reference);
            await Log(new LogMessage(LogSeverity.Info, Sources.command, "Radio deleted"));
        }

        private async void GetFavour(SocketMessage message)
        {
            var refs = MyDatabase.GetRadio(message.GuildId());
            foreach (string reference in refs)
            {
                await message.Channel.SendMessageAsync(reference);
            }
        }

        private async void SetFavour(SocketMessage message)
        {
            string reference = message.Content.Split(' ')[1];
            MyDatabase.AddRadio(message.GuildId(), reference);
            await Log(new LogMessage(LogSeverity.Info, Sources.command, "Reference added"));
        }

        private async void GetRadio(SocketMessage message)
        {
            string key = message.Content.Remove(0, Commands.radio.Length).Trim();
            try
            {
                await message.Channel.SendMessageAsync(RadioReferences.GetRadio(key).Remove(2000));
            }
            catch (ArgumentException)
            {
                await message.Channel.SendMessageAsync("Я не нашел ничего подходящего");
                await Log(new LogMessage(LogSeverity.Error, Sources.command, "Radios not sent"));
                return;
            }
            await Log(new LogMessage(LogSeverity.Info, Sources.command, "Radios sent"));
        }

        private async void WhenJoined(SocketMessage message)
        {
            var users = message.MentionedUsers;
            foreach (var user in users)
            {
                var socketGuildUser = (SocketGuildUser)user;
                await message.Channel.SendMessageAsync($"{socketGuildUser.Username} присоеденился {socketGuildUser.JoinedAt}");
            }
        }

        private async void GetJoke(SocketMessage message)
        {
            string joke = Joke.GetJoke();
            await message.Channel.SendMessageAsync(joke);
            await Log(new LogMessage(LogSeverity.Info, Sources.command, $"GetJoke() : {joke}"));
        }

        private async void MortarQuote(SocketMessage message)
        {
            var quoteChannel = client.GetChannel(546419059231555650);
            IAsyncEnumerable<IReadOnlyCollection<IMessage>> quotes = ((ISocketMessageChannel)quoteChannel).GetMessagesAsync(1000);
            Random random = new();
            List<IMessage> messages = new();
            await foreach (var quote in quotes)
            {
                foreach (var q in quote)
                {
                    messages.Add(q);
                }
            }
            string _quote =  messages.ToArray()[random.Next(messages.Count - 1)].Content;
            await message.Channel.SendMessageAsync(_quote);// работает, и славньо
            await Log(new LogMessage(LogSeverity.Info, Sources.command, $"Отправлена цитата в канал {message.Channel.Name}"));
        }

        private async void OwnerHelp(SocketMessage message)
        {
            string msg = new StreamReader(File.OpenRead(Constants.Constants.ownerMessagePath)).ReadToEnd();
            await message.Author.SendMessageAsync(msg);
        }

        private async void SearchMessages(SocketMessage message)
        {
            string content = message.Content.Remove(0, Commands.search.Length).Trim().TrimEnd();
            ISocketMessageChannel channel = message.Channel;
            IAsyncEnumerable<IReadOnlyCollection<IMessage>> EnumMessages = channel.GetMessagesAsync(1000);
            List<IMessage> findMessages = new();
            await foreach (var messages in EnumMessages)
            {
                foreach (var msg in messages)
                {
                    if (msg.Content.Contains(content))
                    {
                        findMessages.Add(msg);
                    }
                }
            }
            foreach (var msg in findMessages)
            {
                await channel.SendMessageAsync(
                    $"Автор: {msg.Author.Username}\n" +
                    $"Отправлено {msg.CreatedAt.DateTime}\n" +
                    $"Содержание: {msg.Content}");
            }
        }

        private void GiveAdmin(SocketMessage message)
        {
            if (message.MentionedEveryone)
            {
                return;
            }
            string admins = " ";
            foreach (var user in message.MentionedUsers)
            {
                MyDatabase.AddAdmin(message.GuildId(), user.Id);
                admins = admins.Insert(admins.Length, user.Username + " ");
            }
            _ = Log(new(LogSeverity.Info, Sources.internal_function, $"Gave admin to{admins}"));
        }

        private void CommandsHelp(SocketMessage message)
        {
            message.Channel.SendMessageAsync(Commands.commandsHelp);
        }

        private async void RAND_Func(SocketMessage message)
        {
            await message.Channel.SendMessageAsync(
                $"{message.Author.Username}, Артему сегодня повезло, выпало число {new Random().Next(-1000, 1000)}");
            await Log(new(LogSeverity.Info, Sources.command, "Random has been randomed"));
        }

        private async void Desider(SocketMessage message)
        {
            if (new Random().Next(2) % 2 == 0)
            {
                await message.Channel.SendMessageAsync($"{message.Author.Username}, Да");
            }
            else
            {
                await message.Channel.SendMessageAsync($"{message.Author.Username}, Нет");
            }
            await Log(new(LogSeverity.Info, Sources.command, "Bot desided"));
        }

        private async void SPAM_Func(SocketMessage message)
        {
            if (message.Channel.IsReferences())
            {
                _ = message.Channel.SendMessageAsync("отказано");
                return;
            }
            if (message.Content.ToLower().Contains("http"))
            {
                return;
            }
            string[] msg = message.Content.Split(' ');
            int counter = 3;
            try
            {
                counter = Convert.ToInt32(msg[1]);
            }
            catch (FormatException) { }
            catch (IndexOutOfRangeException) { }
            string word = "";
            for (int i = 2; i < msg.Length; i++)
            {
                word += msg[i];
                word += " ";
            }
            try
            {
                for (int i = 0; i < counter; i++)
                {
                    await message.Channel.SendMessageAsync(word);
                }
            }
            catch (ArgumentException)
            {
                await message.Channel.SendMessageAsync($"Правильно \"$spam 5 aboba\", к примеру");
            }
            await Log(new(LogSeverity.Info, Sources.command, $"{word} spammed {counter} times"));
            //Clear(message, msg[0]+msg[1]);
        }

        private async void SHITPOST_Func(SocketMessage message)
        {
            if (message.Channel.IsReferences())
            {
                return;
            }
            await message.Channel.SendMessageAsync(
                $"{message.Author.Username}, {Balaboba.Say(message.Content)}");
        }

        private async void DeleteFunc(SocketMessage message)
        {
            int counter = Convert.ToInt32(message.Content.Substring(Commands.delete.Length).Trim().TrimEnd());
            var messages = message.Channel.GetMessagesAsync(counter);
            await foreach (var msg in messages)
            {
                foreach (var message1 in msg)
                {
                    await message1.DeleteAsync();
                }
            }
        }

        private async void RefModeration(IMessage message)
        {
            ISocketMessageChannel channel = ((ISocketMessageChannel)message.Channel);
            if (channel.IsReferences())
            {
                return;
            }
            if (message.Content.Contains("http"))
            {
                if (message.Content.Contains("gfycat") && message.Content.Contains("gif"))
                {
                    await message.Channel.SendMessageAsync($"||{message.Content}||");
                    await message.DeleteAsync();
                    return;
                }
                ulong refsChannelId = MyDatabase.GetReferencesChannel(Convert.ToString(((SocketGuildChannel)channel).Guild.Id));
                if (refsChannelId == 0)
                {
                    return;
                }
                ulong autorId = message.Author.Id;
                string content = message.Content;
                await message.DeleteAsync();    
                ISocketMessageChannel referencesChannel = 
                    client.GetChannel(refsChannelId) as ISocketMessageChannel;
                if (message.Content.ToLower().Contains("разд") || message.Content.ToLower().Contains("нитро") || message.Content.ToLower().Contains("nitro"))
                {
                    await referencesChannel.SendMessageAsync($"Вероятно, это очередной скам \n ||{content}||");

                    Console.WriteLine($"Переслано скам сообщение от {message.Author.Username}.");
                    return;
                }
                await referencesChannel.SendMessageAsync($"<@{autorId}>: \n\"{content}\""); //(!)
                await Log(new(LogSeverity.Info, Sources.command, $"Message from {message.Author.Username} has been redirected"));
            }
        }

        private async void SendId(SocketMessage message)
        {
            string id = Convert.ToString(message.Author.Id);
            await message.Author.SendMessageAsync($"Твой ID: {id}, id канала {message.Channel.Id}");
            await DeleteMessageAsync(message, enableTimer: false);
            await Log(new(LogSeverity.Info, Sources.command, $"ID send { message.Author.Username }"));
        }

        private async void Clear(SocketMessage message, string command = "")
        {
            string content = message.Content.Remove(0, command.Length).TrimStart().TrimEnd(); //сообщение без слова-команды
            var messages = message.Channel.GetMessagesAsync();
            try
            {
                await foreach (var item in messages)
                {
                    foreach (var msg in item)
                    {
                        if (msg.Content.Contains(content))
                        {
                            await msg.DeleteAsync();
                        }
                    }
                }
            }
            catch (Discord.Net.HttpException e )
            {
                await Log(new LogMessage(LogSeverity.Error, "HttpException", e.Message));
            }
            await Log(new LogMessage(LogSeverity.Info, Sources.command, $"Cleaned channel {message.Channel.Name}"));
        }

        private async void Moderate(SocketMessage message)
        {
            if (message.Channel.IsReferences())
            {
                return;
            }
            var collection = message.Channel.GetMessagesAsync();
            await foreach (var messages in collection)
            {
                foreach (var msg in messages)
                {
                    if (msg.Content.Contains("<@") && msg.Content.Contains(">"))
                    {
                        await msg.DeleteAsync();
                        continue;
                    }
                }
            }
            await message.DeleteAsync();
            await Log(new LogMessage(LogSeverity.Info, Sources.command, $"Channel {message.Channel.Name} cleaned"));
        }

        //private async void Moderate(List<SocketGuildChannel> channels)
        //{
        //    foreach (var channel in channels)
        //    {
        //        ITextChannel textChannel = (ITextChannel)channel;
        //        var messages = textChannel.GetMessagesAsync();
        //        await foreach (var message in messages)
        //        {
        //            foreach (var msg in message)
        //            {
        //                Moderate((SocketMessage)message);
        //            }
        //        }
        //    }
        //}

        #endregion

    }
}
