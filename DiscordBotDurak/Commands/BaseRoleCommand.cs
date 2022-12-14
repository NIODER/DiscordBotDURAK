using DatabaseModel;
using Discord;
using Discord.WebSocket;
using DiscordBotDurak.Data;
using System;

namespace DiscordBotDurak.Commands
{
    internal class BaseRoleCommand : ICommand
    {
        private readonly SocketGuild _guild;
        private readonly SocketRole _role;
        private readonly bool _isSetNewRole;

        public BaseRoleCommand(SocketGuild guild, SocketRole role)
        {
            _guild = guild;
            _isSetNewRole = role is not null;
            _role = role;
        }

        public CommandResult GetResult()
        {
            using var db = new Database();
            var guild = db.GetGuild(_guild.Id);
            if (_isSetNewRole)
            {
                db.BeginTransaction();
                guild.BaseRole = _role.IsEveryone ? Guild.DEFAULT_BASE_ROLE : _role.Id;
                if (!db.CommitAsync().Result)
                {
                    return new CommandResult().WithException(new Exception("Something went wrong."));
                }
                return new CommandResult().WithEmbed(new EmbedBuilder()
                    .WithColor(Color.Blue)
                    .AddField("Successfully:", $"Set new baserole: {_role.Mention}. This role will give every new users.")
                    .WithFooter("base-role command")
                    .WithCurrentTimestamp());
            }
            else
            {
                SocketRole baseRole = guild.BaseRole == Guild.DEFAULT_BASE_ROLE 
                    ? _guild.EveryoneRole 
                    : _guild.GetRole(guild.BaseRole);
                return new CommandResult().WithEmbed(new EmbedBuilder()
                    .WithColor(Color.Blue)
                    .AddField("Current base role:", $"{baseRole.Mention}")
                    .WithFooter("base-role command")
                    .WithCurrentTimestamp());
            }
        }
    }
}
