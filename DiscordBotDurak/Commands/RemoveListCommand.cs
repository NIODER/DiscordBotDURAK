using Discord;
using DiscordBotDurak.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotDurak.Commands
{
    internal class RemoveListCommand : ICommand
    {
        private readonly List<ulong> _lists;
        private readonly int _scope;
        private readonly List<ulong> _channelIds;
        private readonly ulong _guildId;

        public RemoveListCommand(string lists, int scope, ulong guildId, List<ulong> channelIds)
        {
            _lists = Utilities.GetIds(lists);
            _scope = scope;
            _guildId = guildId;
            _channelIds = channelIds;
        }

        public CommandResult GetResult()
        {
            using var db = new Database();
            foreach (var list in _lists)
                if (db.GetSymbolsList(list) is null)
                    _lists.Remove(list);
            db.BeginTransaction();
            if (_scope == 0)
                foreach (var list in _lists)
                    db.DeleteSymbolsListFromGuild(list, _guildId);
            foreach (var channel in _channelIds)
                foreach (var list in _lists)
                    db.DeleteSymbolsListFromChannel(list, channel);
            if (!db.CommitAsync().Result)
            {
                return new CommandResult().WithException(new Exception("Something went wrong."));
            }
            foreach (var channel in _channelIds)
                db.GetUpdatedChannelModeration(channel);
            var result = new StringBuilder("Removed lists:\n");
            _lists.ForEach(l => result.Append($"{l} "));
            return new CommandResult().WithEmbed(new Discord.EmbedBuilder()
                .WithColor(Color.Blue)
                .AddField("Successfully: ", result.ToString())
                .WithFooter("remove-list command")
                .WithCurrentTimestamp());
        }
    }
}
