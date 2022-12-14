using Discord;
using DiscordBotDurak.Data;
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
        private Exception _exception;

        public AddSymbolsCommand(List<ulong> lists, string content, bool excluded)
        {
            _lists = lists;
            _content = content;
            _excluded = excluded;
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

        public CommandResult GetResult()
        {
            if (_exception is not null)
                return new CommandResult().WithException(_exception);
            var symbolId = AddSymbolToLists();
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
