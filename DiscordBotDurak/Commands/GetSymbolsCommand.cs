using DatabaseModel;
using DiscordBotDurak.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordBotDurak.Commands
{
    internal class GetSymbolsCommand : ICommand
    {
        private List<ulong> _listsIds;

        public GetSymbolsCommand(string lists)
        {
            _listsIds = Utilities.GetIds(lists);
        }

        private void DeleteUnexistingLists()
        {
            using var db = new Database();
            foreach (var list in _listsIds)
                if (db.GetSymbolsList(list) is null)
                    _listsIds.Remove(list);
        }

        private List<SymbolsListsToSymbols> GetSymbols()
        {
            using var db = new Database();
            var symbols = new List<SymbolsListsToSymbols>();
            foreach (var listId in _listsIds)
                symbols.AddRange(db.GetSymbolsListsToSymbols(listId));
            return symbols;
        }

        public CommandResult GetResult()
        {
            DeleteUnexistingLists();
            var result = new StringBuilder("[ listId : symbolId : content : excluded ]\n");
            foreach (var slts in GetSymbols())
            {
                result.AppendLine($"[ {slts.ListId} : {slts.SymbolId} : {slts.SymbolNavigation.Content} : {slts.IsExcluded} ]");
            }
            return new CommandResult().WithEmbed(new Discord.EmbedBuilder()
                .WithColor(Discord.Color.Blue)
                .AddField("Symbols", result.ToString())
                .WithFooter("get-symbols command")
                .WithCurrentTimestamp());
        }
    }
}
