using Discord;
using Discord.WebSocket;
using DiscordBotDurak.Data;
using System;

namespace DiscordBotDurak.Commands
{
    internal class SetImmunityCommand : ICommand
    {
        private readonly SocketUser _user;
        private readonly ulong _guildId;
        private readonly bool _enableImmunity;

        public SetImmunityCommand(SocketUser user, bool enableImmunity, ulong guildId)
        {
            _user = user;
            _enableImmunity = enableImmunity;
            _guildId = guildId;
        }

        public CommandResult GetResult()
        {
            using var db = new Database();
            var user = db.GetUser(_guildId, _user.Id);
            db.BeginTransaction();
            user.HasImmunity = _enableImmunity;
            if (!db.CommitAsync().Result)
            {
                return new CommandResult().WithException(new Exception("Something went wrong."));
            }
            return new CommandResult().WithEmbed(new EmbedBuilder()
                .WithColor(Color.Blue)
                .AddField("Immunity setted:", $"{_user.Username} now {(_enableImmunity ? "" : "do not")} have immunity")
                .WithFooter("set-immunity command")
                .WithCurrentTimestamp());
        }
    }
}
