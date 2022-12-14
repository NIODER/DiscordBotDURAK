using Discord;
using DiscordBotDurak.Data;
using DiscordBotDurak.Enum.ModerationModes;
using System;

namespace DiscordBotDurak.Commands
{
    internal class SetModerationCommand : ICommand
    {
        private readonly ulong _channelId;
        private readonly ModerationMode _moderationMode;
        private readonly ulong _listId;

        public SetModerationCommand(ulong channelId, ModerationMode moderationMode, ulong listId)
        {
            _channelId = channelId;
            _moderationMode = moderationMode;
            _listId = listId;
        }

        public CommandResult GetResult()
        {
            using var db = new Database();
            var list = db.GetSymbolsListToChannel(_channelId, _listId);
            if (list is null)
            {
                return new CommandResult().WithException(new Exception($"There is no list with id: {_listId}."));
            }
            db.BeginTransaction();
            list.Moderation = (short)_moderationMode;
            list = db.UpdateSymbolsListToChannel(list);
            db.CommitAsync().Wait();
            db.GetUpdatedChannelModeration(_channelId);
            return new CommandResult().WithEmbed(new EmbedBuilder()
                .WithColor(Color.Blue)
                .AddField("Successfully:", $"Moderation in list {list.SymbolsListNavigation.Title}#{list.ListId} setted to {_moderationMode}")
                .WithFooter("set-moderation command")
                .WithCurrentTimestamp());
        }
    }
}
