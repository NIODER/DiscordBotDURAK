using DatabaseModel;
using Discord;
using Discord.WebSocket;
using DiscordBotDurak.Data;
using DiscordBotDurak.Enum.BotRoles;
using DiscordBotDurak.Enum.ModerationModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBotDurak
{
    public class GuildHandler
    {
        private static readonly List<ulong> _processingGuilds = new();

        public async Task ProcessGuild(SocketGuild guild)
        {
            if (_processingGuilds.Any(id => id == guild.Id))
            {
                Logger.Log(LogSeverity.Info, GetType().Name, $"Guild {guild.Id} already in process");
                return;
            }
            _processingGuilds.Add(guild.Id);
            using var database = new Database();
            if (database.GetGuild(guild.Id) is null)
            {
                var message = await guild.DefaultChannel.SendMessageAsync(embed: new EmbedBuilder()
                    .WithColor(Color.Green)
                    .WithCurrentTimestamp()
                    .WithFooter("configuration")
                    .AddField("Processing guild", "Sending base message, adding guild to database with default settings." +
                    "\n#This text will be removed automaticly.")
                    .Build());
                Logger.Log(LogSeverity.Debug, "ProcessGuild", $"Guild {guild.Id} is not in database");
                var guildOwnerDMChannel = guild.Owner.CreateDMChannelAsync().Result;
                await guildOwnerDMChannel.SendMessageAsync(new Constants().GetOwnerMessage());
                database.BeginTransaction();
                database.AddGuild(new Guild() { GuildId = guild.Id });
                if (database.CommitAsync().Result)
                    Logger.Log(LogSeverity.Info, "GuildHandler", $"Guild {guild.Id} added successfully");
                else Logger.Log(LogSeverity.Error,
                    "GuildHangler", 
                    "Exception occured while guild adding.", 
                    database.Exception);
                await message.ModifyAsync(pr =>
                {
                    pr.Embed = new EmbedBuilder()
                        .WithColor(Color.Green)
                        .WithCurrentTimestamp()
                        .WithFooter("configuration")
                        .AddField("Processing guild", "Ready.\n#This text will be removed automaticly.")
                        .Build();
                });
                _ = Task.Run(() =>
                {
                    Task.Delay(30000).Wait();
                    _ = message.DeleteAsync();
                });
            }
            var processingChannelTasks = guild.TextChannels.Select(ProcessChannel).ToArray();
            var processingUsersTasks = guild.Users.Select(ProcessUser).ToArray();
            Task.WaitAll(processingChannelTasks);
            Task.WaitAll(processingUsersTasks);
            _processingGuilds.Remove(guild.Id);
        }

        public async Task ProcessChannel(SocketGuildChannel channel)
        {
            using var database = new Database();
            if (database.GetChannel(channel.Id) is null)
            {
                database.BeginTransaction();
                database.AddChannel(new Channel()
                {
                    ChannelId = channel.Id,
                    GuildId = channel.Guild.Id
                });
                await database.CommitAsync();
            }
        }

        public async Task ProcessUser(SocketGuildUser user)
        {
            using var db = new Database();
            if (db.GetUser(user.Guild.Id, user.Id) is null)
            {
                db.BeginTransaction();
                if (user.Guild.OwnerId == user.Id)
                {
                    db.AddUser(new GuildUser()
                    {
                        UserId = user.Id,
                        GuildId = user.Guild.Id,
                        Role = (short)BotRole.Owner,
                        HasImmunity = true
                    });
                }
                else
                {
                    db.AddUser(new GuildUser()
                    {
                        UserId = user.Id,
                        GuildId = user.Guild.Id
                    });
                }
                await db.CommitAsync();
            }
            else
            {
                var dbGuild = db.GetGuild(user.Guild.Id);
                if (dbGuild.SpyMode == (short)SpyModesEnum.CollectInfo)
                    return;
                var dbUser = db.GetUser(user.Guild.Id, user.Id);
                if (dbUser.HasImmunity)
                    return;
                var diff = DateTime.Now - dbUser.LastActiveAt.Value;
                Logger.Log(LogSeverity.Debug, "Spy", $"diff: {diff.Days}");
                if (diff.Days < 180)
                    return;
                if (dbGuild.SpyMode == (short)SpyModesEnum.SendTips)
                {
                    Logger.Log(LogSeverity.Info, "Spy", $"User {user.Id} message sent to {user.Guild.OwnerId}. guild id: {user.Guild.Id}.");
                    _ = user.Guild.Owner.SendMessageAsync($"User {user.DisplayName} was last active {diff.Days} days ago.");
                    foreach (var admin in db.GetAllMailing(user.Guild.Id))
                    {
                        _ = user.Guild.GetUser(admin.UserId).SendMessageAsync($"User {user.DisplayName} (guild: {user.Guild.Name}) was last active {diff.Days} days ago.\n" +
                            $"To disable this notifications execute command \"/mailing delete\" and choose yourself in \"user\" parameter.");
                    }
                    db.BeginTransaction();
                    dbUser.LastActiveAt = DateTime.Now;
                    db.UpdateUser(dbUser);
                    await db.CommitAsync();
                }
                else if (dbGuild.SpyMode == (short)SpyModesEnum.DeleteUsers)
                {
                    Logger.Log(LogSeverity.Info, "Spy", $"User {user.Id} was kicked from guild {user.Guild.Id}.");
                    _ = user.KickAsync($"You have been inactive in the {user.Guild.Name} guild for {diff.Days} days");
                    db.BeginTransaction();
                    db.DeleteUser(dbUser);
                    await db.CommitAsync();
                }
                else
                    Logger.Log(LogSeverity.Warning, "Spy", "Guild spy mode is not a CollextInfo or SendTips or DeleteUser.");
            }
        }
    }
}
