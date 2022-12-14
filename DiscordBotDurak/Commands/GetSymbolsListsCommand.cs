using Discord;
using DiscordBotDurak.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordBotDurak.Commands
{
    internal class GetSymbolsListsCommand : ICommand
    {
        private readonly ulong _channelId;
        private readonly ulong _guildId;
        private readonly long _scope;

        public GetSymbolsListsCommand(ulong channelId, ulong guildId, long scope)
        {
            _channelId = channelId;
            _guildId = guildId;
            _scope = scope;
        }

        private List<SymbolObject> GetChannelSymbolObjects()
        {
            using var db = new Database();
            var symbols = new List<SymbolObject>();
            symbols.AddRange(db.GetChannelBanSymbols(_channelId));
            return symbols;
        }

        private List<SymbolObject> GetGuildSymbols()
        {
            using var db = new Database();
            var symbolsLists = db.GetGuildSymbolsLists(_guildId);
            var symbols = new List<SymbolObject>();
            foreach (var list in symbolsLists)
            {
                symbols.AddRange(db.GetSymbolsListsToSymbols(list.ListId)
                    .Select(sl => new SymbolObject(sl.SymbolId, sl.SymbolNavigation.Content, sl.IsExcluded, sl.ListId)));
            }
            return symbols;
        }

        public CommandResult GetResult()
        {
            var result = new StringBuilder("[ list : symbol_id : content : excluded ]\n");
            if (_scope == 0)
                foreach (var symbol in GetGuildSymbols())
                    result.AppendLine($"[ {symbol.ListId} : {symbol.SymbolId} : {symbol.Content} : {symbol.IsExcluded} ]");
            else
                foreach (var symbol in GetChannelSymbolObjects())
                    result.AppendLine($"[ {symbol.ListId} : {symbol.SymbolId} : {symbol.Content} : {symbol.IsExcluded} ]");
            return new CommandResult().WithEmbed(new EmbedBuilder()
                .WithColor(Color.Blue)
                .AddField("Symbols:", result.ToString())
                .WithFooter("get-list command")
                .WithCurrentTimestamp());
        }
    }
}
