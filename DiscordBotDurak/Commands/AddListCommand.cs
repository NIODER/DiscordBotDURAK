using DatabaseModel;
using Discord;
using DiscordBotDurak.Data;
using DiscordBotDurak.Enum.ModerationModes;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;

namespace DiscordBotDurak.Commands
{
    internal class AddListCommand : ICommand
    {
        private readonly long _scope;
        private readonly ulong? _list;
        private ModerationMode? _moderationMode;
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
        private SymbolsList DublicateSymbolsList(SymbolsList symbolsList, ulong guildId)
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
            using var db = new Database();
            var symbolsList = new SymbolsList();

            // не указан id списка символов, создаем новый
            if (_list is null)
            {
                if (_moderationMode == ModerationMode.OnlyResend && _resendChannelId is null)
                {
                    Logger.Log(LogSeverity.Info, GetType().Name, "No resend message specified for resend moderation mode.");
                    return new CommandResult().WithException("Ypu need to attach resend channel for this moderation mode.");
                }
                db.BeginTransaction();
                symbolsList = db.CreateSymbolsList(_title);
                if (_scope == 0)
                {
                    db.AddSymbolsListToGuild(_guildId, symbolsList);
                }
                else
                {
                    db.AddSymbolsListToChannel(symbolsList.ListId, _channelId, _moderationMode ?? ModerationMode.NonModerated, _resendChannelId);
                }
                db.CommitAsync().Wait();
                return new CommandResult().WithEmbed(new EmbedBuilder()
                    .WithColor(Color.Blue)
                    .AddField("Succesfully:", $"List added id: {symbolsList.ListId}")
                    .WithFooter("list command")
                    .WithCurrentTimestamp());
            }
            // id указан, значит изменяем старый
            else
            {
                if (_list == 0)
                {
                    return new CommandResult().WithException("List id overflow exception.");
                }
                symbolsList = db.GetSymbolsList(_list.Value);
                if (db.GetGuildSymbolsLists(_guildId) is null)
                {
                    db.AddSymbolsListToGuild(_guildId, symbolsList);
                }
                // если указан scope channel, то добавляем список в канал
                if (_scope == 1)
                {
                    if (!db.GetChannel(_channelId).SymbolsLists.Contains(symbolsList))
                    {
                        db.AddSymbolsListToChannel(symbolsList.ListId, _channelId, _moderationMode.GetValueOrDefault(), _resendChannelId);
                    }
                    else
                    {
                        db.BeginTransaction();
                        var sltc = db.GetSymbolsListToChannel(_channelId, symbolsList.ListId);
                        if (_moderationMode.HasValue)
                        {
                            sltc.Moderation = (short)_moderationMode.Value;
                        }
                        if (_resendChannelId.HasValue)
                        {
                            sltc.ResendChannelId = _resendChannelId.Value;
                        }
                        db.UpdateSymbolsListToChannel(sltc);
                        db.CommitAsync().Wait();
                    }
                }
                if (_title is null)
                {
                    return new CommandResult().WithEmbed(new EmbedBuilder()
                            .WithColor(Color.Blue)
                            .AddField("Successfully:", $"Added list {symbolsList.ListId} to {(_scope == 0 ? "guild" : "channel")} {_channelId}")
                            .WithFooter("list command")
                            .WithCurrentTimestamp());
                }
                else
                {
                    db.BeginTransaction();
                    // если список делять несколько гильдий, то дублируем его для изменения в текущей
                    if (symbolsList.Guilds.Count > 1)
                    {
                        symbolsList = DublicateSymbolsList(symbolsList, _guildId);
                    }
                    symbolsList.Title = _title;
                    db.CommitAsync().Wait();
                }

                return new CommandResult().WithEmbed(new EmbedBuilder()
                    .WithColor(Color.Blue)
                    .AddField("Successfully:", $"Updated. New ListId: {symbolsList.ListId}.\n" +
                        $"Symbols count: {symbolsList.Symbols.Count}.\n" +
                        $"Title: {symbolsList.Title}.")
                    .AddField("Channels:", symbolsList.Channels.Aggregate(", ", (s, c) => s += c.ChannelId))
                    .WithFooter("list command")
                    .WithCurrentTimestamp());
            }
        }
    }
}
