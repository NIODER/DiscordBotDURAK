using DatabaseModel;
using Discord;
using Discord.WebSocket;
using DiscordBotDurak.CommandHandlers;
using DiscordBotDurak.Data;
using DiscordBotDurak.Enum.BotRoles;
using DiscordBotDurak.Enum.ModerationModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordBotDurak.Commands
{
    internal class InfoCommand : ICommand
    {
        private readonly InfoSlashCommandHandler.Type _type;
        private readonly SocketGuildUser _user;
        private readonly SocketChannel _channel;
        private readonly SocketGuild _guild;
        private readonly ulong _listId;

        public InfoCommand(InfoSlashCommandHandler.Type type, SocketGuildUser user, SocketChannel channel, SocketGuild guild, ulong listId)
        {
            _type = type;
            _user = user;
            _channel = channel;
            _guild = guild;
            _listId = listId;
        }

        private string GetModeration(short moderation)
        {
            var moderations = new List<ModerationMode>()
            {
                ModerationMode.NonModerated,
                ModerationMode.OnlyWarnings,
                ModerationMode.OnlyResend,
                ModerationMode.OnlyDelete
            };
            return moderations
                .Where(m => Utilities.CodeContains(moderation, m))
                .Select(m => m.ToString())
                .Aggregate((prev, next) => $"{prev} {next}");
        }

        private StringBuilder GetGuildInfo(SocketGuild guild)
        {
            using var db = new Database();
            var dbGuild = db.GetGuild(guild.Id);
            if (dbGuild == null)
                return null;
            var info = new StringBuilder($"Info about guild {guild.Name}:\n");
            info.AppendLine($"guild id: {dbGuild.GuildId}");
            info.AppendLine($"spy mode: {(SpyModesEnum)dbGuild.SpyMode}");
            var baseRole = dbGuild.BaseRole == Guild.DEFAULT_BASE_ROLE 
                ? guild.EveryoneRole.Mention 
                : guild.GetRole(dbGuild.BaseRole).Mention;
            info.AppendLine($"base role: {baseRole}");
            var immunityRole = dbGuild.ImmunityRole == Guild.DEFAULT_IMMUNITY_ROLE 
                ? "none" : 
                guild.GetRole(dbGuild.ImmunityRole.Value).Mention;
            info.AppendLine($"immunity role: {immunityRole}");
            return info;
        }

        private StringBuilder GetChannelInfo(SocketGuildChannel channel)
        {
            if (channel is null)
                return null;
            using var db = new Database();
            var dbChannel = db.GetChannel(channel.Id);
            if (dbChannel is null)
                return null;
            var info = new StringBuilder($"Info about channel {channel.Name}:\n");
            info.AppendLine($"channel id: {dbChannel.ChannelId}");
            info.AppendLine($"moderation: {GetModeration(dbChannel.Moderation)}");
            info.AppendLine($"warning message: {dbChannel.Warning}");
            return info;
        }

        private StringBuilder GetUserInfo(SocketGuildUser user)
        {
            using var db = new Database();
            var dbUser = db.GetUser(user.Guild.Id, user.Id);
            if (dbUser is null)
                return null;
            var info = new StringBuilder($"Info about user {user.Mention}:\n");
            info.AppendLine($"user id: {dbUser.UserId}");
            info.AppendLine($"Last active at: {dbUser.LastActiveAt}");
            info.AppendLine($"Bot role: {(BotRole)dbUser.Role}");
            info.AppendLine($"Has immunity: {dbUser.HasImmunity}");
            info.AppendLine($"Invited by: {dbUser.Invited ?? "unknown"}");
            info.AppendLine($"Introduced as: {dbUser.Introduced ?? "unknown"}");
            return info;
        }

        private StringBuilder GetListInfo(ulong listId, ulong guildId)
        {
            var info = new StringBuilder($"Info about list {listId}:\n");
            using var db = new Database();
            var sl = db.GetSymbolsList(listId);
            if (sl is null)
            {
                info.AppendLine("List does not exists.");
                return info;
            }
            info.AppendLine($"Title: {sl.Title},");
            info.AppendLine($"Symbols count: {sl.Symbols.Count},");
            info.AppendLine($"Current guild channels count: {db.GetChannelsContainsList(guildId, listId).Count()},");
            info.AppendLine($"Guilds count: {sl.Guilds.Count}");
            info.AppendLine($"Symbols (\"e\" if excluded, overwise \"i\"):\n{db.GetSymbolObjects(listId)
                .Select(s => $"{s.Content}({(s.IsExcluded ? "e" : "i")})").Aggregate((s1, s2) => $"{s1}, {s2}")}");
            return info;
        }

        public CommandResult GetResult()
        {
            var info = _type switch
            {
                InfoSlashCommandHandler.Type.Guild => GetGuildInfo(_guild),
                InfoSlashCommandHandler.Type.Channel => GetChannelInfo(_channel as SocketGuildChannel),
                InfoSlashCommandHandler.Type.List => GetListInfo(_listId, _guild.Id),
                InfoSlashCommandHandler.Type.User => GetUserInfo(_user),
                _ => null
            };
            if (info is null)
            {
                return new CommandResult().WithException(_type switch
                {
                    InfoSlashCommandHandler.Type.Channel => new Exception("Channel is not a guild or message channel."),
                    _ => new Exception("Something went wrong.")
                });
            }
            return new CommandResult().WithEmbed(new EmbedBuilder()
                .WithColor(Color.Blue)
                .AddField("Result:", info.ToString())
                .WithFooter("info command")
                .WithCurrentTimestamp())
                .WithEphemeral(true);
        }
    }
}
