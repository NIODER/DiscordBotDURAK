using DatabaseModel;
using Discord;
using DiscordBotDurak.Data;
using DiscordBotDurak.Enum.ModerationModes;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordBotDurak.Commands
{
    internal class RemoveSymbolCommand : ICommand
    {
        private readonly List<ulong> _lists;
        private readonly ulong _symbolId;
        private readonly ulong _guildId;

        public RemoveSymbolCommand(string lists, ulong symbolId, ulong guildId)
        {
            _lists = Utilities.GetIds(lists);
            _symbolId = symbolId;
            _guildId = guildId;
        }

        /// <summary>
        /// Replaces <paramref name="symbolsList"/> by new symbols list in guild with id: <paramref name="guildId"/>.
        /// </summary>
        /// <param name="symbolsList">Symbols list need to replace.</param>
        /// <param name="symbolId">Forbidden symbol id need to remove.</param>
        /// <param name="guildId">Guild where need to replace symbols list.</param>
        /// <returns>New symbols list without symbol with id: <paramref name="symbolId"/>.</returns>
        private SymbolsList DublicateSymbolsList(SymbolsList symbolsList, ulong symbolId, ulong guildId)
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

            symbols.RemoveAll(s => s.SymbolId == symbolId);

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
            var replaces = new Dictionary<ulong, ulong>(); // old : new list ids
            foreach (var list in _lists)
            {
                var symbolsList = db.GetSymbolsList(list);
                if (symbolsList.Guilds.Count > 1)
                {
                    var symbolsList1 = DublicateSymbolsList(symbolsList, _symbolId, _guildId);
                    replaces.Add(symbolsList.ListId, symbolsList1.ListId);
                }
                else db.RemoveSymbolFromSymbolsList(list, _symbolId);
            }
            var result = new StringBuilder($"Symbol: {_symbolId} removed from lists:\n");
            _lists.ForEach(l => result.Append($"{l} "));
            if (replaces.Count > 0)
            {
                result.AppendLine();
                result.AppendLine($"This lists was replaced by equvalent without symbol {_symbolId}:");
                result.AppendLine("[ old list id : new list id ]");
                replaces.ToList().ForEach(r => result.AppendLine($"[ {r.Key} : {r.Value} ]"));
            }
            return new CommandResult().WithEmbed(new EmbedBuilder()
                .WithColor(Color.Blue)
                .AddField("Successfully:", result.ToString())
                .WithFooter("remove-symbol command")
                .WithCurrentTimestamp());
        }
    }
}
