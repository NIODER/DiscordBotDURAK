using System;
using System.Collections.Generic;
using System.Linq;
using CyberShoke.Objects;
using Discord.WebSocket;

namespace DiscordBotDURAK
{
    public static class Extentions
    {
        public static bool isDirect(this ISocketMessageChannel channel) => channel.GetType().ToString() == "Discord.WebSocket.SocketDMChannel";
        public static bool InDatabase(this ISocketMessageChannel channel) => channel.ChannelType() != ChannelSeverity.NoSuchChannel;
        public static string GuildId(this SocketMessage message) => Convert.ToString(((SocketGuildChannel)message.Channel).Guild.Id);
        public static bool IsReferences(this ISocketMessageChannel channel) => ChannelSeverity.References == MyDatabase.ChannelType(channel);
        public static bool IsAuthorAdmin(this SocketMessage message) => message.Author.isAdmin(((SocketGuildChannel)message.Channel).Guild.Id);
        public static Server GetRandom(this IEnumerable<Server> list) => list.ElementAt(new Random().Next(list.Count() - 1));
        public static string Info(this Server server) => $"{server.category} {server.country} {server.players}/{server.maxplayers}\nconnect {server.ip}:{server.port}";
    }
}
