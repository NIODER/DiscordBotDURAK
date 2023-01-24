using DatabaseModel;
using Discord;
using DiscordBotDurak.Data;
using DiscordBotDurak.Enum.ModerationModes;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;

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
        private Exception _exception;

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

        public CommandResult GetResult()
        {
            using var db = new Database();
            var symbolsList = new SymbolsList();

            if (_moderationMode is null)
            {
                if (_resendChannelId is not null)
                {
                    _moderationMode = ModerationMode.OnlyResend;
                }
                else
                {
                    _moderationMode = ModerationMode.NonModerated;
                }
            }

            if (_list is null)
            {
                if (_moderationMode is not null)
                {
                    if (_moderationMode == ModerationMode.OnlyResend && _resendChannelId is null)
                    {
                        Logger.Log(LogSeverity.Info, GetType().Name, "No resend message specified for resend moderation mode.");
                        return new CommandResult().WithException("Ypu need to attach resend channel for this moderation mode.");
                    }
                }
                db.BeginTransaction();
                symbolsList = db.CreateSymbolsList(_title);
                if (_scope == 0)
                {
                    db.AddSymbolsListToGuild(_guildId, symbolsList);
                }
                else
                {
                    db.AddSymbolsListToChannel(symbolsList.ListId, _channelId, _moderationMode.Value, _resendChannelId);
                }
                return new CommandResult().WithEmbed(new EmbedBuilder()
                    .WithColor(Color.Blue)
                    .AddField("Succesfully:", $"List added id: {symbolsList.ListId}")
                    .WithFooter("list command")
                    .WithCurrentTimestamp());
            }
            else
            {
                symbolsList = db.GetSymbolsList(_list.Value);
                if (_title is null && _resendChannelId is null && _moderationMode is null)
                {
                    return new CommandResult().WithEmbed(new EmbedBuilder()
                        .WithColor(Color.Blue)
                        .AddField("Successfully:", $"List {symbolsList.ListId}.\n" +
                            $"Symbols count: {symbolsList.Symbols.Count}.\n" +
                            $"Title: {symbolsList.Title}.\n" +
                            $"Channels count: {symbolsList.Channels.Count}.")
                        .AddField("Channels:", symbolsList.Channels.Aggregate(", ", (s, c) => s += c.ChannelId))
                        .WithFooter("list command")
                        .WithCurrentTimestamp());
                }
                db.BeginTransaction();
                SymbolsListsToChannels slToChannel = db.GetSymbolsListToChannel(_channelId, _list.Value);
                if (_title is not null)
                {
                    symbolsList.Title = _title;
                }
                if (_resendChannelId is not null)
                {
                    slToChannel.ResendChannelId = _resendChannelId;
                }
                if (_moderationMode is not null)
                {
                    slToChannel.Moderation = (short)_moderationMode.Value;
                }
                db.CommitAsync().Wait();
                return new CommandResult().WithEmbed(new EmbedBuilder()
                    .WithColor(Color.Blue)
                    .AddField("Successfully:", $"List {symbolsList.ListId}.\n" +
                        $"Symbols count: {symbolsList.Symbols.Count}.\n" +
                        $"Title: {symbolsList.Title}.\n" +
                        $"Channels count: {symbolsList.Channels.Count}.")
                    .AddField("Channels:", symbolsList.Channels.Aggregate(", ", (s, c) => s += c.ChannelId))
                    .WithFooter("list command")
                    .WithCurrentTimestamp());
            }

            //if (_scope == 0)
            //{
            //    symbolsList.Channels = db.GetGuild(_guildId).Channels;
            //}
            //else
            //{
            //    symbolsList.Channels = new() { db.GetChannel(_channelId) };
            //}


            //SymbolsList list;
            //if (_list == 0)
            //{
            //    db.BeginTransaction();
            //    list = db.CreateSymbolsList(_title);
            //    db.CommitAsync().Wait();
            //}
            //else list = db.GetSymbolsList(_list);
            //if (list is null)
            //{
            //    return new CommandResult().WithException(new Exception($"There is no list with id: {_list}"));
            //}
            //if (_moderationMode == ModerationMode.OnlyResend && _resendChannelId == null)
            //{
            //    _exception = new Exception("You need to specify resend channel if ypu want set \"resend\" moderation mode.");
            //    Logger.Log(LogSeverity.Info, GetType().Name, "No resend channel for resend moderation.");
            //    return new CommandResult().WithException(_exception);
            //}
            //if (!db.GetGuildSymbolsLists(_guildId).Contains(list))
            //{
            //    db.BeginTransaction();
            //    db.AddSymbolsListToGuild(_guildId, list);
            //    if (!db.CommitAsync().Result)
            //    {
            //        _exception = new Exception("Something goes wrong. Can't add list to guild.");
            //        Logger.Log(LogSeverity.Error, GetType().Name, "Cannot add list to guild.", db.Exception);
            //    }
            //}
            //if (_scope == 1)
            //{
            //    if (!db.GetChannel(_channelId).SymbolsLists.Contains(list))
            //    {
            //        db.BeginTransaction();
            //        db.AddSymbolsListToChannel(list.ListId, _channelId, _moderationMode ?? ModerationMode.NonModerated, _resendChannelId);
            //        if (!db.CommitAsync().Result)
            //        {
            //            _exception = new Exception("Something goes wrong. Can't add list to channel.");
            //            Logger.Log(LogSeverity.Error, GetType().Name, "Cannot add list to db.", db.Exception);
            //        }
            //        db.GetUpdatedChannelModeration(_channelId);
            //    }
            //    else
            //    {
            //        db.BeginTransaction();
            //        var dbSlToChannel = db.GetSymbolsListToChannel(_channelId, _list);
            //        if (_moderationMode is not null)
            //        {
            //            dbSlToChannel.Moderation = (short)_moderationMode.Value;
            //        }
            //        db.UpdateSymbolsListToChannel(dbSlToChannel);
            //        if (!db.CommitAsync().Result)
            //            _exception = new Exception("Something goes wrong. Can't set moderation.");
            //    }
            //}
            //if (_exception is null)
            //    return new CommandResult().WithEmbed(new EmbedBuilder()
            //        .WithColor(Color.Blue)
            //        .AddField("Successfully:", $"Added list: {list.Title}#{list.ListId}")
            //        .WithFooter("add-list command")
            //        .WithCurrentTimestamp());
            //else return new CommandResult().WithException(_exception);
        }
    }
}
