using Discord;
using Discord.WebSocket;
using DiscordBotDurak.Data;
using DiscordBotDurak.Enum.ModerationModes;
using System;

namespace DiscordBotDurak.Commands
{
    internal class SetSpyModeCommand : ICommand
    {
        private readonly SpyModesEnum _spyMode;
        private readonly SocketGuild _socketGuild;
        private readonly bool _isSetMode;

        public SetSpyModeCommand(SpyModesEnum spyMode, SocketGuild socketGuild, bool isSetMode)
        {
            _spyMode = spyMode;
            _socketGuild = socketGuild;
            _isSetMode = isSetMode;
        }

        public CommandResult GetResult()
        {
            using var db = new Database();
            var guild = db.GetGuild(_socketGuild.Id);
            if (_isSetMode)
            {
                db.BeginTransaction();
                guild.SpyMode = (short)_spyMode;
                if (!db.CommitAsync().Result)
                {
                    return new CommandResult().WithException(new Exception("Something went wrong."));
                }
                return new CommandResult().WithEmbed(new EmbedBuilder()
                    .WithColor(Color.Blue)
                    .AddField("Spy mode changed to:", _spyMode)
                    .WithFooter("spy-mode command")
                    .WithCurrentTimestamp());
            }
            else
            {
                return new CommandResult().WithEmbed(new EmbedBuilder()
                    .WithColor(Color.Blue)
                    .AddField("Current spy mode:", (SpyModesEnum)guild.SpyMode)
                    .WithFooter("spy-mode command")
                    .WithCurrentTimestamp());
            }
        }
    }
}
