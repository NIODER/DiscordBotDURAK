using DatabaseModel;
using Discord;
using DiscordBotDurak.Data;
using DiscordBotDurak.Enum.ModerationModes;
using System.Collections.Generic;

namespace DiscordBotDurak.Commands
{
    internal class AddListCommand : ICommand
    {
        private readonly long _scope;
        private readonly ulong? _list;
        private readonly ModerationMode? _moderationMode;
        private readonly string _title;
        private readonly ulong _guildId;
        private readonly ulong _channelId;
        private readonly ulong? _resendChannelId;

        public AddListCommand(long scope, ulong? list, ModerationMode? moderationMode, string title, ulong guildId, ulong channelId, ulong? resendChannelId)
        {
            _scope = scope;
            _list = list;
            _moderationMode = moderationMode;
            _title = title;
            _guildId = guildId;
            _channelId = channelId;
            _resendChannelId = resendChannelId;
        }

        /// <summary>
        /// Replaces <paramref name="symbolsList"/> by new symbols list in guild with id: <paramref name="guildId"/>.
        /// </summary>
        /// <param name="symbolsList">Symbols list need to replace.</param>
        /// <param name="guildId">Guild where need to replace symbols list.</param>
        /// <returns>New symbols list without symbol with id: <paramref name="symbolId"/>.</returns>
        private static SymbolsList DublicateSymbolsList(SymbolsList symbolsList, ulong guildId)
        {
            using var db = new Database();
            var symbols = db.GetSymbolsListsToSymbols(symbolsList.ListId);
            var channels = db.GetChannelsContainsList(guildId, symbolsList.ListId);
            var channelsToLists = new List<SymbolsListsToChannels>();
            channels.ForEach(c => channelsToLists.Add(db.GetSymbolsListToChannel(c.ChannelId, symbolsList.ListId)));
            db.BeginTransaction();
            foreach (var channel in channels)
            {
                db.DeleteSymbolsListFromChannel(symbolsList.ListId, channel.ChannelId);
            }
            db.CommitAsync().Wait();

            db.BeginTransaction();
            var symbolsList1 = db.CreateSymbolsList(symbolsList.Title);
            db.CommitAsync().Wait();

            db.BeginTransaction();
            foreach (var symbol in symbols)
            {
                db.AddSymbolToBanwordList(symbolsList1.ListId, symbol.SymbolId, symbol.IsExcluded);
            }
            db.CommitAsync().Wait();

            db.BeginTransaction();
            db.DeleteSymbolsListFromGuild(symbolsList.ListId, guildId);
            db.AddSymbolsListToGuild(guildId, symbolsList1);
            db.CommitAsync().Wait();

            db.BeginTransaction();
            foreach (var ctl in channelsToLists)
            {
                db.AddSymbolsListToChannel(symbolsList1.ListId, ctl.ChannelId, (ModerationMode)ctl.Moderation);
            }
            db.CommitAsync().Wait();

            return symbolsList1;
        }

        public CommandResult GetResult()
        {
            bool createNew = _list is null;
            bool properties = _moderationMode is not null ||
                _resendChannelId is not null;
            using var db = new Database();
            if (createNew)
            {
                db.BeginTransaction();
                var newList = db.CreateSymbolsList(_title);
                if (_moderationMode is not null)
                {
                    if (_resendChannelId is null && _moderationMode == ModerationMode.OnlyResend)
                    {
                        db.RollBack();
                        return new CommandResult().WithException($"You must set resend channel for moderation mode \"{_moderationMode}\".");
                    }
                }
                db.AddSymbolsListToGuild(_guildId, newList);
                db.CommitAsync().Wait();
                if (_scope == 1)
                {
                    db.BeginTransaction();
                    db.AddSymbolsListToChannel(newList.ListId, _channelId, _moderationMode.Value, _resendChannelId);
                    db.CommitAsync().Wait();
                }
                return new CommandResult().WithEmbed(new EmbedBuilder()
                    .WithColor(Color.Blue)
                    .WithCurrentTimestamp()
                    .WithFooter("list command")
                    .AddField("List created", $"Code: {newList.ListId}#{newList.Title}."));
            }
            else
            {
                db.BeginTransaction();
                var list = db.GetSymbolsList(_list.Value);
                if (list is null)
                {
                    return new CommandResult().WithException($"There is no list with id: {_list}.");
                }
                if (properties)
                {
                    if (_title is not null) // if we changing shared list
                    {
                        list.Title = _title;
                        if (list.Guilds.Count > 1)
                        {
                            list = DublicateSymbolsList(list, _guildId);
                        }
                        else
                        {
                            db.AddSymbolsListToGuild(_guildId, list);
                        }
                    }
                    if (_moderationMode is not null)
                    {
                        if (_resendChannelId is null && _moderationMode == ModerationMode.OnlyResend)
                        {
                            db.RollBack();
                            return new CommandResult().WithException($"You must set resend channel for moderation mode \"{_moderationMode}\".");
                        }
                        var sltc = db.GetSymbolsListToChannel(_channelId, list.ListId);
                        if (sltc is null)
                        {
                            db.BeginTransaction();
                            db.AddSymbolsListToChannel(list.ListId, _channelId, _moderationMode.Value, _resendChannelId);
                            db.CommitAsync().Wait();
                            sltc = db.GetSymbolsListToChannel(_channelId, list.ListId);
                        }
                        sltc.Moderation = (short)_moderationMode;
                        sltc.ResendChannelId = _resendChannelId;
                        db.UpdateSymbolsListToChannel(sltc);
                        db.CommitAsync().Wait();
                    }
                }
                else
                {
                    db.AddSymbolsListToGuild(_guildId, list);
                    db.CommitAsync().Wait();
                    if (_scope == 1)
                    {
                        db.BeginTransaction();
                        db.AddSymbolsListToChannel(list.ListId, _channelId, _moderationMode.Value, _resendChannelId);
                        db.CommitAsync().Wait();
                    }
                }
                return new CommandResult().WithEmbed(new EmbedBuilder()
                    .WithColor(Color.Blue)
                    .WithCurrentTimestamp()
                    .WithFooter("list command")
                    .AddField("List added", $"Code: {list.ListId}#{list.Title}."));
            }
        }
    }
}
