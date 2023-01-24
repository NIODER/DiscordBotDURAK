using DatabaseModel;
using Discord;
using DiscordBotDurak.Data;
using DiscordBotDurak.Enum.ModerationModes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotDurak.Commands
{
    internal class AddSymbolsCommand : ICommand
    {
        private readonly List<ulong> _lists;
        private readonly string _content;
        private readonly bool _excluded;
        private readonly ulong _guildId;
        private Exception _exception;

        public AddSymbolsCommand(List<ulong> lists, string content, bool excluded, ulong guildId)
        {
            _lists = lists;
            _content = content;
            _excluded = excluded;
            _guildId = guildId;
        }

        public AddSymbolsCommand(Exception exception)
        {
            _exception = exception;
        }

        private ulong AddSymbolToLists()
        {
            using var db = new Database();
            db.BeginTransaction();
            var symbol = db.FindSymbol(_content) ?? db.AddSymbol(_content);
            db.CommitAsync().Wait();
            db.BeginTransaction();
            foreach (var list in _lists)
            {
                var symbolsList = db.GetSymbolsList(list);
                db.AddSymbolToBanwordList(symbolsList, symbol, _excluded);
            }
            if (!db.CommitAsync().Result)
            {
                _exception = new Exception("Something went wrong.");
            }
            return db.FindSymbol(_content).SymbolId;
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
            if (_exception is not null)
                return new CommandResult().WithException(_exception);
            ulong symbolId = 0;
            var replaces = new Dictionary<ulong, ulong>(); // old : new list ids
            using var db = new Database();
            foreach (var list in _lists)
            {
                var symbolsList = db.GetSymbolsList(list);
                if (symbolsList.Guilds.Count > 1)
                {
                    var symbolsList1 = DublicateSymbolsList(symbolsList, _guildId);
                    replaces.Add(symbolsList.ListId, symbolsList1.ListId);
                }
                else
                {
                    var symbol = db.FindSymbol(_content) ?? db.AddSymbol(_content);
                    symbolId = symbol.SymbolId;
                    db.AddSymbolToBanwordList(symbolsList, symbol, _excluded);
                }
            }
            var result = new StringBuilder($"Added symbol id: {symbolId} in lists: ");
            foreach (var list in _lists)
            {
                result.Append(list);
                result.Append(' ');
            }
            if (_exception is not null)
                return new CommandResult().WithException(_exception);
            else
                return new CommandResult().WithEmbed(new Discord.EmbedBuilder()
                    .WithColor(Color.Blue)
                    .AddField("Successfully:", result.ToString())
                    .WithFooter("add-symbol command")
                    .WithCurrentTimestamp());
        }
    }
}
