using Discord.WebSocket;
using DiscordBotDurak.Commands;
using DiscordBotDurak.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscordBotDurak.CommandHandlers
{
    internal class AddSymbolSlashCommandHandler : ICommandHandler
    {
        private readonly string _content;
        private readonly bool _exluded;
        private readonly string _list;

        public AddSymbolSlashCommandHandler(SocketSlashCommand command)
        {
            _content = (string)command.Data.Options.First(op => op.Name == "content").Value;
            _exluded = (bool)command.Data.Options.First(op => op.Name == "excluded").Value;
            _list = (string)command.Data.Options.First(op => op.Name == "list").Value;
        }

        private void RemoveUnexistingLists(List<ulong> lists)
        {
            using var db = new Database();
            foreach (var list in lists)
                if (db.GetSymbolsList(list) is null)
                    lists.Remove(list);
        }

        public ICommand CreateCommand()
        {
            var lists = Utilities.GetIds(_list);
            RemoveUnexistingLists(lists);
            if (lists.Count == 0)
                return new AddSymbolsCommand(new Exception("No exinsting lists found."));
            else return new AddSymbolsCommand(lists, _content, _exluded);
        }
    }
}
