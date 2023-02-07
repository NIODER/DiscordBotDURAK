using DatabaseModel;
using Discord;
using Discord.WebSocket;
using DiscordBotDurak.Data;
using DiscordBotDurak.Enum.BotRoles;
using DiscordBotDurak.Enum.ModerationModes;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBotDurak
{
    public static class GuildHandler
    {
        public static async void ProcessGuild(SocketGuild guild)
        {
            var message = guild.DefaultChannel.SendMessageAsync("Processing your guild...");
            using var database = new Database();
            if (database.GetGuild(guild.Id) is null)
            {
                Logger.Log(LogSeverity.Debug, "ProcessGuild", $"Guild is not in database");
                var guildOwnerDMChannel = guild.Owner.CreateDMChannelAsync().Result;
                await guildOwnerDMChannel.SendMessageAsync(new Constants().GetOwnerMessage());
                database.BeginTransaction();
                database.AddGuild(new Guild() { GuildId = guild.Id });
                if (database.CommitAsync().Result)
                    Logger.Log(LogSeverity.Info, "GuildHandler", "Guild added successfully");
                else Logger.Log(LogSeverity.Error,
                    "GuildHangler", 
                    "Exception occured while guild adding.", 
                    database.Exception);
            }
            var processingChannelTasks = guild.TextChannels.Select(ProcessChannel).ToArray();
            var processingUsersTasks = guild.Users.Select(ProcessUser).ToArray();
            Task.WaitAll(processingChannelTasks);
            Task.WaitAll(processingUsersTasks);
            _ = message.Result.DeleteAsync();
            var message1 = await guild.DefaultChannel.SendMessageAsync("Ready!");
            await Task.Delay(30000);
            _ =message1.DeleteAsync();
        }

        public static async Task ProcessChannel(SocketGuildChannel channel)
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

        public static async Task ProcessUser(SocketGuildUser user)
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
