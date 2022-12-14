using DatabaseModel;
using Discord;
using Discord.WebSocket;
using DiscordBotDurak.Data;
using DiscordBotDurak.Enum.ModerationModes;
using System.Collections.Generic;
using System.Linq;

namespace DiscordBotDurak
{
    public static class Moderator
    {
        public static void SendWarning(IMessage socketMessage)
        {
            using var db = new Database();
            string warningMessage = db.GetChannel(socketMessage.Channel.Id).Warning;
            Logger.Log(LogSeverity.Info, "Moderator", $"Warning message sent, channel id: {socketMessage.Channel.Id}");
            _ = socketMessage.Channel.SendMessageAsync(text: warningMessage);
        }

        /// <summary>
        /// Resend message content from <paramref name="socketGuildChannel"/> to resend channel.
        /// </summary>
        /// <param name="socketGuildChannel"></param>
        /// <param name="content"></param>
        /// <param name="banwordListId"></param>
        /// <returns>true if message was successfully resent</returns>
        public static bool Resend(SocketGuildChannel socketGuildChannel,
            string content,
            string authorNickname,
            ulong? resendChannelId)
        {
            using var db = new Database();
            if (resendChannelId is null)
            {
                _ = socketGuildChannel.Guild.Owner.SendMessageAsync(
                    $"Banword detected in guild {socketGuildChannel.Guild.Name} " +
                    $"in channel {socketGuildChannel.Name} " +
                    $"and channel moderation mode contains resend. But there is " +
                    $"no resend channel setted for this channel. Set resend channel or " +
                    $"change moderation mode.");
                return false;
            }
            if (socketGuildChannel.Guild.GetChannel((ulong)resendChannelId) is not ISocketMessageChannel resendChannel)
            {
                _ = socketGuildChannel.Guild.Owner.SendMessageAsync(
                    $"Banword detected in guild {socketGuildChannel.Guild.Name} " +
                    $"in channel {socketGuildChannel.Name} " +
                    $"and channel moderation mode contains resend. But there is " +
                    $"no resend channel with id {resendChannelId} in this guild " +
                    $"or channel is not message channel. " +
                    $"Set existing message resend channel.");
                return false;
            }
            _ = resendChannel.SendMessageAsync($"{authorNickname}:\n{content}");
            return true;
        }

        public static SymbolsListsToChannels GetSymbolToChannel(ulong guildId, ulong channelId, string content)
        {
            using var db = new Database();
            var channel = db.GetChannel(channelId);
            if (channel is null)
            {
                db.BeginTransaction();
                db.AddChannel(new Channel()
                {
                    ChannelId = channelId,
                    GuildId = guildId
                });
                db.CommitAsync().Wait();
                return null;
            }
            var symbols = new List<SymbolObject>();
            var symbolsListsToChannel = db.GetSymbolsListsToChannel(channelId);
            foreach (var sltc in symbolsListsToChannel)
                symbols.AddRange(db.GetChannelBanSymbols(sltc.ChannelId));
            symbols = symbols.Where(s => content.Contains(s.Content)).ToList();
            if (symbols.Count == 0)
                return null;
            foreach (var symbol in symbols)
                if (symbol.IsExcluded)
                    return null;
            SymbolsListsToChannels symbolListToChannel1 = null;
            foreach (var sltc in symbolsListsToChannel)
            {
                if (sltc.Moderation > (symbolListToChannel1?.Moderation ?? (short)ModerationMode.NonModerated))
                    symbolListToChannel1 = sltc;
            }
            return symbolListToChannel1;
        }

        /// <summary>
        /// Moderation without responding something.
        /// </summary>
        public static void Moderate(IMessage message, SocketGuildChannel socketGuildChannel, bool withWarnings)
        {
            var symbolListToChannel = GetSymbolToChannel(socketGuildChannel.Guild.Id, socketGuildChannel.Id, message.Content);
            if (symbolListToChannel is null)
                return;
            var moderation = (ModerationMode)symbolListToChannel.Moderation;
            switch (moderation)
            {
                case ModerationMode.OnlyWarnings:
                    if (withWarnings)
                        SendWarning(message);
                    break;
                case ModerationMode.OnlyResend:
                    bool resent = Resend(socketGuildChannel,
                        message.Content,
                        message.Author.Username,
                        symbolListToChannel.ResendChannelId);
                    if (resent) _ = message.DeleteAsync();
                    break;
                case ModerationMode.OnlyDelete:
                    _ = message.DeleteAsync();
                    break;
                case ModerationMode.NonModerated:
                    return;
            }
        }

        /// <summary>
        /// Tries to find banword.
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="message">message content</param>
        /// <returns>zero if <paramref name="message"/> does not contains one 
        /// of channel with id:"<paramref name="channelId"/>" banwords, 
        /// overwise returns symbols list id</returns>
        public static ulong ContainsBanword(ulong channelId, string message)
        {
            using var db = new Database();
            var channelBanwords = db.GetChannelBanSymbols(channelId);
            foreach (var banword in channelBanwords)
                if (message.Contains(banword.Content))
                    return banword.IsExcluded ? 0 : banword.ListId;
            return 0;
        }
    }
}
