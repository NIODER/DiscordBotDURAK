using CyberShoke.Objects;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBotDurak
{
    public static class Extentions
    {
        public static Task RespondAsync(this SocketSlashCommand command, CommandResult commandResult)
        {
            if (commandResult?.Exception is null)
            {
                return command?.RespondAsync(text: commandResult?.Text,
                    isTTS: commandResult?.IsTTS ?? false,
                    embed: commandResult?.Embed,
                    options: commandResult?.RequestOptions,
                    allowedMentions: commandResult?.AllowedMentions,
                    components: commandResult?.MessageComponent,
                    embeds: commandResult?.Embeds);
            }
            else
            {
                var embed = new EmbedBuilder()
                    .WithColor(Color.Red)
                    .AddField("Error oocured", commandResult.Exception.Message);
                return command.RespondAsync(embed: embed.Build());
            }
        }
        public static Task RespondAsync(this SocketMessage message, CommandResult commandResult)
        {
            if (commandResult.Exception is null)
            {
                return message.Channel.SendMessageAsync(text: commandResult.Text,
                    isTTS: commandResult.IsTTS,
                    embed: commandResult.Embed,
                    options: commandResult.RequestOptions,
                    allowedMentions: commandResult.AllowedMentions,
                    components: commandResult.MessageComponent,
                    embeds: commandResult.Embeds);
            }
            else
            {
                var embed = new EmbedBuilder()
                    .WithColor(Color.Red)
                    .AddField("Error occured", commandResult.Exception.Message);
                return message.Channel.SendMessageAsync(embed: embed.Build());
            }
        }

        public static Task RespondAsync(this SocketMessageComponent message, CommandResult commandResult)
        {
            if (commandResult.Exception is null)
            {
                return message.RespondAsync(text: commandResult.Text,
                    isTTS: commandResult.IsTTS,
                    embed: commandResult.Embed,
                    options: commandResult.RequestOptions,
                    allowedMentions: commandResult.AllowedMentions,
                    components: commandResult.MessageComponent,
                    embeds: commandResult.Embeds);
            }
            else
            {
                var embed = new EmbedBuilder()
                    .WithColor(Color.Red)
                    .AddField("Error occured", commandResult.Exception.Message);
                return message.RespondAsync(embed: embed.Build());
            }
        }

        public static Task SendMessageAsync(this ISocketMessageChannel channel, CommandResult commandResult)
        {
            if (commandResult.Exception is null)
            {
                return channel.SendMessageAsync(text: commandResult.Text,
                    isTTS: commandResult.IsTTS,
                    embed: commandResult.Embed,
                    options: commandResult.RequestOptions,
                    allowedMentions: commandResult.AllowedMentions,
                    components: commandResult.MessageComponent,
                    embeds: commandResult.Embeds);
            }
            else
            {
                var embed = new EmbedBuilder()
                    .WithColor(Color.Red)
                    .AddField("Error occured", commandResult.Exception.Message);
                return channel.SendMessageAsync(embed: embed.Build());
            }
        }

        public static Task ModifyOriginalResponseAsync(this SocketSlashCommand slashCommand, CommandResult commandResult)
        {
            return null;
        }

        public static long Next(this Random random, long min, long max)
        {
            long r = random.Next((Int32)(min >> 32), (Int32)(max >> 32));
            r <<= 32;
            r |= (long)random.Next((Int32)min, (Int32)max);
            return r;
        }

        public static T GetRandom<T>(this IEnumerable<T> list) => list.ElementAt(new Random().Next(list.Count() - 1));
        public static string Info(this Server server) => $"{server.category} region: {server.country} {server.players}/{server.maxplayers}\nconnect {server.ip}:{server.port}";
    }
}
