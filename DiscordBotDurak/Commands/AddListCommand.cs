using DatabaseModel;
using Discord;
using DiscordBotDurak.Data;
using DiscordBotDurak.Enum.ModerationModes;
using System;

namespace DiscordBotDurak.Commands
{
    internal class AddListCommand : ICommand
    {
        private readonly long _scope;
        private readonly ulong _list;
        private readonly ModerationMode _moderationMode;
        private readonly string _title;
        private readonly ulong _guildId;
        private readonly ulong _channelId;
        private Exception _exception;

        public AddListCommand(long scope, ulong list, ModerationMode moderationMode, string title, ulong guildId, ulong channelId)
        {
            _scope = scope;
            _list = list;
            _moderationMode = moderationMode;
            _title = title;
            _guildId = guildId;
            _channelId = channelId;
        }

        private void AddListToChannel(SymbolsList symbolsList)
        {
            using var db = new Database();
        }

        public CommandResult GetResult()
        {
            using var db = new Database();
            SymbolsList list;
            if (_list == 0)
            {
                db.BeginTransaction();
                list = db.CreateSymbolsList(_title);
                db.CommitAsync().Wait();
            }
            else list = db.GetSymbolsList(_list);
            if (list is null)
            {
                return new CommandResult().WithException(new Exception($"There is no list with id: {_list}"));
            }
            if (!db.GetGuildSymbolsLists(_guildId).Contains(list))
            {
                db.BeginTransaction();
                db.AddSymbolsListToGuild(_guildId, list);
                if (!db.CommitAsync().Result)
                {
                    _exception = new Exception("Something goes wrong. Can't add list to guild.");
                    Logger.Log(LogSeverity.Error, GetType().Name, "Cannot add list to guild.", db.Exception);
                }
            }
            if (_scope == 1)
            {
                if (!db.GetChannel(_channelId).SymbolsLists.Contains(list))
                {
                    db.BeginTransaction();
                    db.AddSymbolsListToChannel(list.ListId, _channelId, _moderationMode);
                    if (!db.CommitAsync().Result)
                    {
                        _exception = new Exception("Something goes wrong. Can't add list to channel.");
                        Logger.Log(LogSeverity.Error, GetType().Name, "Cannot add list to db.", db.Exception);
                    }
                    db.GetUpdatedChannelModeration(_channelId);
                }
            }
            if (_exception is null)
                return new CommandResult().WithEmbed(new EmbedBuilder()
                    .WithColor(Color.Blue)
                    .AddField("Successfully:", $"Added list: {list.Title}#{list.ListId}")
                    .WithFooter("add-list command")
                    .WithCurrentTimestamp());
            else return new CommandResult().WithException(_exception);
        }
    }
}
