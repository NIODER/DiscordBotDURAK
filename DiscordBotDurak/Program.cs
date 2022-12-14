using DatabaseModel;
using Discord;
using Discord.WebSocket;
using DiscordBotDurak.CommandHandlers;
using DiscordBotDurak.Data;
using DiscordBotDurak.Enum.BotRoles;
using DiscordBotDurak.Enum.ModerationModes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DiscordBotDurak
{
    class Program
    {
        public const string configPath = "../../../resources/config.json";
        private DiscordSocketClient _client;
        private List<SocketApplicationCommand> _commands;

        static Task Main(string[] args) => new Program().MainAsync(args);

        public async Task MainAsync(string[] args)
        {
            LogSeverity logSeverity = (LogSeverity)Convert.ToInt32(args[0]);
            var logger = Logger.Instance(logSeverity);
            var config = new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.Guilds |
                                 GatewayIntents.GuildMembers |
                                 GatewayIntents.GuildBans |
                                 GatewayIntents.GuildIntegrations |
                                 GatewayIntents.GuildVoiceStates |
                                 GatewayIntents.GuildPresences |
                                 GatewayIntents.GuildMessages |
                                 GatewayIntents.GuildMessageTyping |
                                 GatewayIntents.MessageContent
            };
            _client = new DiscordSocketClient(config);
            _client.Log += logger.LogAsync;
            _client.Ready += OnReady;
            _client.ChannelCreated += OnChannelCreated;
            _client.ChannelDestroyed += OnChannelDestroyed;
            _client.JoinedGuild += OnGuildJoined;
            _client.GuildAvailable += OnGuildAvailable;
            _client.GuildUnavailable += OnGuildUnavailable;
            _client.LeftGuild += OnLeftGuild;
            _client.RoleDeleted += OnRoleDeleted;
            _client.UserBanned += OnUserBanned;
            _client.UserJoined += OnUserJoined;
            _client.UserUnbanned += OnUserUnbanned;
            _client.UserVoiceStateUpdated += OnUserVoiceStateUpdated;
            _client.MessageReceived += OnMessageReceivedAsync;
            _client.SelectMenuExecuted += OnSelectMenuExecuted;
            _client.SlashCommandExecuted += OnSlashCommandExecuted;
            _client.GuildMembersDownloaded += OnMembersDownloaded;
            _client.UserUpdated += OnUserUpdated;
            _commands = new List<SocketApplicationCommand>();
            await _client.LoginAsync(
                TokenType.Bot,
                JObject.Parse(File.ReadAllText(configPath))["token"]?.ToString());
            await _client.StartAsync();
            await _client.SetStatusAsync(UserStatus.DoNotDisturb);
            while (true)
            {
                string command = Console.ReadLine();
                if (command == "stop")
                {
                    return;
                }
                if (command == "exit")
                {
                    OnExit();
                    return;
                }
            }
        }

        private async Task OnUserUpdated(SocketUser old, SocketUser user)
        {
            Logger.Log(LogSeverity.Debug, "OnUserUpdated", $"user {user.Username} сделал какую-то хуйню");
        }

        private async Task OnChannelCreated(SocketChannel channel)
        {
            Logger.Log(LogSeverity.Info, "OnChannelCreated", "Channel created intent activated.");
            if (channel is not SocketGuildChannel and ISocketMessageChannel)
                return;
            _ = ((ISocketMessageChannel)channel).SendMessageAsync(embed: new EmbedBuilder()
                .WithColor(Color.Green)
                .WithTitle("Info")
                .AddField("New channel detected:", "Channel added in database with default settings.")
                .WithFooter("DiscordBotDurak moderation")
                .WithCurrentTimestamp().Build());
            await GuildHandler.ProcessChannel(channel as SocketGuildChannel);
        }

        private async Task OnChannelDestroyed(SocketChannel channel)
        {
            Logger.Log(LogSeverity.Info, "OnChannelDestroyed", "Channel destroyed intent activated.");
            if (channel is not SocketGuildChannel and ISocketMessageChannel)
                return;
            await Task.Run(() =>
            {
                using var db = new Database();
                var dbChannel = db.GetChannel(channel.Id);
                if (dbChannel is null)
                    return;
                db.DeleteChannel(dbChannel);
            });
        }

        private async Task OnGuildAvailable(SocketGuild guild)
        {
            Logger.Log(LogSeverity.Info, "OnGuildAvailable", "Guild available intent activated.");
            await Task.Run(() =>
            {
                foreach (var command in new SlashCommandCreator().GetAllSlashCommands())
                    _ = guild.CreateApplicationCommandAsync(command.Build());
            });
        }

        private async Task OnGuildUnavailable(SocketGuild guild)
        {
            Logger.Log(LogSeverity.Info, "OnGuildAvailable", "Guild unavailable intent activated.");
            await guild.DeleteApplicationCommandsAsync();
        }

        private async Task OnLeftGuild(SocketGuild guild)
        {
            Logger.Log(LogSeverity.Info, "OnLeftGuild", "Left guild intent activated.");
            await guild.DeleteApplicationCommandsAsync();
            await Task.Run(() =>
            {
                using var db = new Database();
                var dbGuild = db.GetGuild(guild.Id);
                db.DeleteGuild(dbGuild);
            });
        }

        private async Task OnRoleDeleted(SocketRole role)
        {
            Logger.Log(LogSeverity.Info, "OnRoleDeleted", "Role deleted intent activated.");
            await Task.Run(async () =>
            {
                using var db = new Database();
                var dbGuild = db.GetGuild(role.Guild.Id);
                if (dbGuild.BaseRole == role.Id)
                    _ = role.Guild.Owner.SendMessageAsync($"In guild {role.Guild.Name} role {role.Name} was deleted. " +
                        $"This role has been set as base role, so new base role is @everyone.");
                db.BeginTransaction();
                dbGuild.BaseRole = Guild.DEFAULT_BASE_ROLE;
                db.UpdateGuild(dbGuild);
                await db.CommitAsync();
            });
        }

        private async Task OnUserBanned(SocketUser user, SocketGuild guild)
        {
            Logger.Log(LogSeverity.Info, "OnUserBanned", "User banned intent activated.");
            await Task.Run(async () =>
            {
                using var db = new Database();
                var dbUser = db.GetUser(guild.Id, user.Id);
                if (dbUser is null)
                {
                    dbUser = db.AddUser(new GuildUser()
                    {
                        UserId = user.Id,
                        GuildId = guild.Id,
                        Role = (short)BotRole.Ban
                    });
                }
                else
                {
                    db.BeginTransaction();
                    dbUser.Role = (short)BotRole.Ban;
                    db.UpdateUser(dbUser);
                    await db.CommitAsync();
                }
            });
        }

        private async Task OnUserJoined(SocketGuildUser user)
        {
            Logger.Log(LogSeverity.Info, "OnUserJoined", "User joined intent activated.");
            await Task.Run(() =>
            {
                using var db = new Database();
                var dbUser = db.GetUser(user.Guild.Id, user.Id);
                if (dbUser is null)
                {
                    db.AddUser(new GuildUser()
                    {
                        UserId = user.Id,
                        GuildId = user.Guild.Id
                    });
                }
            });
        }

        private async Task OnUserUnbanned(SocketUser user, SocketGuild guild)
        {
            Logger.Log(LogSeverity.Info, "OnUserUnbanned", "User unbanned intent activated.");
            await Task.Run(() =>
            {
                using var db = new Database();
                db.BeginTransaction();
                var dbUser = db.GetUser(guild.Id, user.Id);
                dbUser.Role = (short)BotRole.User;
                db.CommitAsync().Wait();
            });
        }

        private async Task OnUserVoiceStateUpdated(SocketUser user, SocketVoiceState socketVoiceState, SocketVoiceState socketVoiceState1)
        {
            Logger.Log(LogSeverity.Info, "OnUserVoiceStateUpdated", "User voice state updated intent activated.");
            await Task.Run(() =>
            {
                using var db = new Database();
                db.BeginTransaction();
                var dbUser = db.GetUser((socketVoiceState.VoiceChannel ?? socketVoiceState1.VoiceChannel).Guild.Id, user.Id);
                if (dbUser is null)
                {
                    db.AddUser(new GuildUser()
                    {
                        UserId = user.Id,
                        GuildId = socketVoiceState.VoiceChannel.Guild.Id
                    });
                    return;
                }
                dbUser.LastActiveAt = DateTime.UtcNow;
                db.UpdateUser(dbUser);
                db.CommitAsync().Wait();
            });
        }

        private void OnExit()
        {
            Logger.Log(LogSeverity.Info, "Program", "All guild commands will be deleted.");
            foreach (var guild in _client.Guilds)
                _ = guild.DeleteApplicationCommandsAsync();
        }

        private async Task OnReady()
        {
            await _client.SetStatusAsync(UserStatus.Online);
        }

        private async Task OnGuildJoined(SocketGuild guild)
        {
            Logger.Log(LogSeverity.Debug, "Program", "Guild joined.");
            await guild.DownloadUsersAsync();
        }

        private Task OnMembersDownloaded(SocketGuild guild)
        {
            Logger.Log(LogSeverity.Debug, "Program", "OnMembersDownloaded intent activated.");
            GuildHandler.ProcessGuild(guild);
            return Task.CompletedTask;
        }

        private async Task OnSlashCommandExecuted(SocketSlashCommand slashCommand)
        {
            Logger.Log(LogSeverity.Info, GetType().Name, $"slash command {slashCommand.CommandName} executed");
            await Task.Run(async () =>
            {
                if (slashCommand.Channel is not SocketGuildChannel) return;
                ICommandHandler commandHandler = slashCommand.CommandName switch
                {
                    "random-number" => new RandomNumberCommandHandler(slashCommand),
                    "random-user" => new RandomUserCommandHandler(slashCommand),
                    "random-decide" => new RandomDecideCommandHandler(),
                    "random-distribute" => new RandomDistributionCommandHandler(slashCommand),
                    "spam" => new SpamSlashCommandHandler(slashCommand),
                    "stop" => new StopCommandHandler(slashCommand),
                    "cybershoke" => new CybershokeCommandHandler(),
                    "delete" => new DeleteCommandHandler(slashCommand),
                    "set-privelege" => new SetPrivelegeSlashCommandHandler(slashCommand),
                    "get-lists" => new GetSymbolsListsSlashCommandHandler(slashCommand),
                    "get-symbols" => new GetSymbolsSlashCommandHandler(slashCommand),
                    "add-list" => new AddListSlashCommandHandler(slashCommand),
                    "add-symbol" => new AddSymbolSlashCommandHandler(slashCommand),
                    "remove-list" => new RemoveListSlashCommandHandler(slashCommand),
                    "remove-symbol" => new RemoveSymbolSlashCommandHandler(slashCommand),
                    "set-moderation" => new SetModerationCommandHandler(slashCommand),
                    "warning-message" => new WarningMessageSlashCommandHandler(slashCommand),
                    "base-role" => new BaseRoleSlashCommandHandler(slashCommand),
                    "set-immunity" => new SetImmunitySlashCommandHandler(slashCommand),
                    "spy-mode" => new SetSpyModeSlashCommandHandler(slashCommand),
                    "info" => new InfoSlashCommandHandler(slashCommand),
                    _ => throw new ArgumentOutOfRangeException()
                };
                var command = commandHandler.CreateCommand();
                var commandResult = command.GetResult();
                await slashCommand.RespondAsync(commandResult);
            });
            //return Task.CompletedTask;
        }

        public static async Task OnMessageReceivedAsync(IMessage socketMessage)
        {
            if (socketMessage.Channel is SocketGuildChannel channel)
            {
                await Task.Run(() =>
                {
                    if (socketMessage.Author.IsBot) return;
                    if (socketMessage.Author.IsWebhook) return;
                    Moderator.Moderate(socketMessage, channel, true);
                });
            }
        }

        private async Task OnSelectMenuExecuted(SocketMessageComponent component)
        {
            await Task.Run(async () =>
            {
                var commandHandler = new CybershokeCommandHandler(component);
                var command = commandHandler.CreateCommand();
                await component.RespondAsync(command.GetResult());
            });
        }
    }
}
