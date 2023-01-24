using DatabaseModel;
using Discord;
using Discord.WebSocket;
using DiscordBotDurak.CommandHandlers;
using DiscordBotDurak.Data;
using DiscordBotDurak.Enum.BotRoles;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DiscordBotDurak
{
    class Program
    {
        public static string resourcesPath;
        private DiscordSocketClient _client;

        static Task Main(string[] args) => new Program().MainAsync(args);

        public async Task MainAsync(string[] args)
        {
            resourcesPath = args[1];
            LogSeverity logSeverity = (LogSeverity)Convert.ToInt32(args[0]);
            var logger = Logger.Instance(logSeverity);
            var config = new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.Guilds |
                                 GatewayIntents.GuildMembers |
                                 GatewayIntents.GuildBans |
                                 GatewayIntents.GuildPresences |
                                 GatewayIntents.GuildIntegrations |
                                 GatewayIntents.GuildVoiceStates |
                                 GatewayIntents.GuildMessages |
                                 GatewayIntents.GuildMessageTyping |
                                 GatewayIntents.MessageContent |
                                 GatewayIntents.DirectMessages,
                HandlerTimeout = 4000
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
            await _client.LoginAsync(
                TokenType.Bot,
                JObject.Parse(File.ReadAllText(resourcesPath + "/config.json"))["token"]?.ToString());
            await _client.StartAsync();
            await _client.SetStatusAsync(UserStatus.DoNotDisturb);
            while (true)
            {
                string command = Console.ReadLine();
                if (command == "stop")
                {
                    break;
                }
                if (command == "exit")
                {
                    await OnExit();
                    break;
                }
            }
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

        private Task OnGuildAvailable(SocketGuild guild)
        {
            Logger.Log(LogSeverity.Info, "OnGuildAvailable", "Guild available intent activated.");
            _ = Task.Run(() =>
            {
                using var db = new Database();
                if (db.GetGuild(guild.Id) is null)
                {
                    GuildHandler.ProcessGuild(guild);
                }
            });
            _ = Task.Run(async () =>
            {
                foreach (var command in new SlashCommandCreator().GetAllSlashCommands())
                {
                    guild.CreateApplicationCommandAsync(command.Build(), new RequestOptions()).Wait();
                    await Task.Delay(1500);
                }
            });
            return Task.CompletedTask;
        }

        private async Task OnGuildUnavailable(SocketGuild guild)
        {
            Logger.Log(LogSeverity.Info, "OnGuildAvailable", "Guild unavailable intent activated.");
            await guild.DeleteApplicationCommandsAsync();
        }

        private Task OnLeftGuild(SocketGuild guild)
        {
            Logger.Log(LogSeverity.Info, "OnLeftGuild", "Left guild intent activated.");
            _ = guild.DeleteApplicationCommandsAsync();
            _ = Task.Run(() =>
            {
                using var db = new Database();
                var dbGuild = db.GetGuild(guild.Id);
                db.DeleteGuild(dbGuild);
            });
            return Task.CompletedTask;
        }

        private Task OnRoleDeleted(SocketRole role)
        {
            Logger.Log(LogSeverity.Info, "OnRoleDeleted", "Role deleted intent activated.");
            _ = Task.Run(async () =>
            {
                using var db = new Database();
                var dbGuild = db.GetGuild(role.Guild.Id);
                if (dbGuild.BaseRole == role.Id)
                {
                    _ = role.Guild.Owner.SendMessageAsync($"In guild {role.Guild.Name} role {role.Name} was deleted. " +
                        $"This role has been set as base role, so new base role is @everyone.");
                    db.BeginTransaction();
                    dbGuild.BaseRole = Guild.DEFAULT_BASE_ROLE;
                    db.UpdateGuild(dbGuild);
                    await db.CommitAsync();
                }
            });
            return Task.CompletedTask;
        }

        private Task OnUserBanned(SocketUser user, SocketGuild guild)
        {
            Logger.Log(LogSeverity.Info, "OnUserBanned", "User banned intent activated.");
            _ = Task.Run(async () =>
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
            return Task.CompletedTask;
        }

        private Task OnUserJoined(SocketGuildUser user)
        {
            Logger.Log(LogSeverity.Info, "OnUserJoined", "User joined intent activated.");
            _ = Task.Run(async () =>
            {
                using var db = new Database();
                db.BeginTransaction();
                var dbUser = db.GetUser(user.Guild.Id, user.Id);
                if (dbUser is null)
                {
                    var qMessageId = user.SendMessageAsync($"Здравствуйте, я бот-ассистент гильдии {user.Guild.Name}.\n" +
                    $"Ответьте на два вопроса, они нужны для идентификации вас администраторами этой гильдии.\n" +
                    $"1. Представтесь.").Result.Id;
                    db.AddUser(new GuildUser()
                    {
                        UserId = user.Id,
                        GuildId = user.Guild.Id,
                        QMessageId = qMessageId
                    });
                    db.CommitAsync().Wait();
                    Logger.Log(LogSeverity.Info, "OnUserJoined", $"Base role setted to user: {user.Id}");
                }
                else
                {
                    var baseRole = db.GetGuild(user.Guild.Id).BaseRole;
                    if (baseRole != Guild.DEFAULT_BASE_ROLE)
                    {
                        await user.AddRoleAsync(baseRole);
                    }
                }
            });
            return Task.CompletedTask;
        }

        private Task OnUserUnbanned(SocketUser user, SocketGuild guild)
        {
            Logger.Log(LogSeverity.Info, "OnUserUnbanned", "User unbanned intent activated.");
            _ = Task.Run(() =>
            {
                using var db = new Database();
                db.BeginTransaction();
                var dbUser = db.GetUser(guild.Id, user.Id);
                dbUser.Role = (short)BotRole.User;
                db.CommitAsync().Wait();
            });
            return Task.CompletedTask;
        }

        private Task OnUserVoiceStateUpdated(SocketUser user, SocketVoiceState socketVoiceState, SocketVoiceState socketVoiceState1)
        {
            Logger.Log(LogSeverity.Info, "OnUserVoiceStateUpdated", "User voice state updated intent activated.");
            _ = Task.Run(async () =>
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
                await db.CommitAsync();
            });
            return Task.CompletedTask;
        }

        private async Task OnExit()
        {
            Logger.Log(LogSeverity.Info, "Program", "All guild commands will be deleted.");
            foreach (var guild in _client.Guilds)
                await guild.DeleteApplicationCommandsAsync();
        }

        private async Task OnReady()
        {
            await _client.SetStatusAsync(UserStatus.Online);
        }

        private Task OnGuildJoined(SocketGuild guild)
        {
            Logger.Log(LogSeverity.Info, "Program", $"Guild joined {guild.Id}.");
            _ = guild.DownloadUsersAsync();
            Logger.Log(LogSeverity.Debug, "OnGuildJoined", $"Users count {guild.Users.Count}");
            return Task.CompletedTask;
        }

        private Task OnMembersDownloaded(SocketGuild guild)
        {
            Logger.Log(LogSeverity.Debug, "Program", "OnMembersDownloaded intent activated.");
            Logger.Log(LogSeverity.Debug, "OnMembersDownloaded", $"Members count {guild.Users.Count}");
            GuildHandler.ProcessGuild(guild);
            return Task.CompletedTask;
        }

        private Task OnSlashCommandExecuted(SocketSlashCommand slashCommand)
        {
            Logger.Log(LogSeverity.Info, GetType().Name, $"slash command {slashCommand.CommandName} executed");
            _ = slashCommand.RespondAsync("Processing...");
            _ = Task.Run(() =>
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
                    "list" => new AddListSlashCommandHandler(slashCommand),
                    "add-symbol" => new AddSymbolSlashCommandHandler(slashCommand),
                    "remove-list" => new RemoveListSlashCommandHandler(slashCommand),
                    "remove-symbol" => new RemoveSymbolSlashCommandHandler(slashCommand),
                    "set-moderation" => new SetModerationCommandHandler(slashCommand),
                    "warning-message" => new WarningMessageSlashCommandHandler(slashCommand),
                    "base-role" => new BaseRoleSlashCommandHandler(slashCommand),
                    "set-immunity" => new SetImmunitySlashCommandHandler(slashCommand),
                    "spy-mode" => new SetSpyModeSlashCommandHandler(slashCommand),
                    "info" => new InfoSlashCommandHandler(slashCommand),
                    "help" => new HelpCommandHandler(slashCommand),
                    _ => throw new ArgumentOutOfRangeException("CommandName")
                };
                var command = commandHandler.CreateCommand();
                var commandResult = command.GetResult();
                _ = slashCommand.ModifyOriginalResponseAsync(pr =>
                {
                    if (commandResult.Exception is null)
                    {
                        pr.Content = new(commandResult.Text);
                        pr.Embed = new(commandResult.Embed);
                        pr.Embeds = new(commandResult.Embeds);
                        pr.Components = new(commandResult.MessageComponent);
                    }
                    else
                    {
                        pr.Embed = new(new EmbedBuilder()
                            .WithColor(Color.Red)
                            .AddField("Error occured", commandResult.Exception.Message).Build());
                    }
                });
            });
            return Task.CompletedTask;
        }

        public async Task OnMessageReceivedAsync(IMessage message)
        {
            if (message.Author.IsBot || message.Author.IsWebhook)
            {
                return;
            }
            if (message.Channel is SocketGuildChannel channel)
            {
                _ = Task.Run(() =>
                {
                    if (message.Author.IsBot) return;
                    if (message.Author.IsWebhook) return;
                    Moderator.Moderate(message, channel, true);
                });
            }
            if (message.Channel is SocketDMChannel dMChannel)
            {
                ulong? qMessageId = message?.Reference?.MessageId.Value;
                if (qMessageId == null)
                {
                    _ = message.Channel.SendMessageAsync("Ответьте на сообщение с вопросом для получения роли.");
                    return;
                }
                using var db = new Database();
                var dbUser = db.GetUserByQuestion(qMessageId.Value, message.Author.Id);
                db.BeginTransaction();
                if (dbUser.Introduced == "unknown")
                {
                    dbUser.Introduced = message.Content;
                    qMessageId = message.Channel.SendMessageAsync("2. Кто вас пригласил в гилдьдию?").Result.Id;
                    dbUser.QMessageId = qMessageId;
                    db.UpdateUser(dbUser);
                    await db.CommitAsync();
                } 
                else if (dbUser.Invited == "unknown")
                {
                    Logger.Log(LogSeverity.Debug, "add user", "invited added");
                    dbUser.Invited = message.Content;
                    db.UpdateUser(dbUser);
                    await db.CommitAsync();
                    var guild = _client.GetGuild(dbUser.GuildId);
                    var user = guild.GetUser(message.Author.Id);
                    ulong bRoleId = dbUser.GuildNavigation.BaseRole;
                    if (bRoleId == 0)
                    {
                        return;
                    }
                    var bRole = guild.GetRole(dbUser.GuildNavigation.BaseRole);
                    if (user == null)
                    {
                        Logger.Log(LogSeverity.Error, "AddBaseRole", $"No user with id {message.Author.Id} in guild {guild.Id}, question message id {qMessageId}");
                        return;
                    }
                    await user.AddRoleAsync(bRole);
                    _ = message.Channel.SendMessageAsync($"Вам выдана базовая роль {bRole.Name} в гильдии {guild.Name}.");
                }
            }
        }

        private Task OnSelectMenuExecuted(SocketMessageComponent component)
        {
            _ = Task.Run(() =>
            {
                var commandHandler = new CybershokeCommandHandler(component);
                var command = commandHandler.CreateCommand();
                _ = component.RespondAsync(command.GetResult());
            });
            return Task.CompletedTask;
        }
    }
}
