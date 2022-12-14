using Discord;
using DiscordBotDurak.Data;
using System;

namespace DiscordBotDurak.Commands
{
    internal class WarningMessageCommand : ICommand
    {
        private readonly ulong _channelId;
        private readonly string _message;

        public WarningMessageCommand(ulong channelId, string message)
        {
            _channelId = channelId;
            _message = message;
        }

        public CommandResult GetResult()
        {
            using var db = new Database();
            var channel = db.GetChannel(_channelId);
            if (_message is not null)
            {
                channel.Warning = _message;
                db.BeginTransaction();
                db.UpdateChannel(channel);
                if (!db.CommitAsync().Result)
                    return new CommandResult().WithException(new Exception("Somethong went wrong"));
                return new CommandResult().WithEmbed(new EmbedBuilder()
                    .WithColor(Color.Blue)
                    .AddField("Successfully:", $"Warning message in this channel set to: \"{_message}\""));
            }
            else
            {
                return new CommandResult().WithEmbed(new EmbedBuilder()
                    .WithColor(Color.Blue)
                    .AddField("Successfully:", $"Warning message in this channel : \"{channel.Warning}\""));
            }
        }
    }
}
