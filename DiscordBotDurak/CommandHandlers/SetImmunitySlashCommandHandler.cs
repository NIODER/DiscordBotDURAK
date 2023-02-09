using Discord.WebSocket;
using DiscordBotDurak.Commands;
using DiscordBotDurak.Verification;
using System.Linq;

namespace DiscordBotDurak.CommandHandlers
{
    internal class SetImmunitySlashCommandHandler : AvailableToAdmin, ICommandHandler
    {
        private readonly SocketUser _user;
        private readonly bool _enableImmunity;
        private readonly ulong _guildId;

        public SetImmunitySlashCommandHandler(SocketSlashCommand command)
        {
            _user = (SocketUser)command.Data.Options.First(op => op.Name == "user").Value;
            _enableImmunity = (bool)command.Data.Options.First(op => op.Name == "enable-immunity").Value;
            _guildId = command.GuildId.Value;
        }

        public ICommand CreateCommand()
        {
            return new SetImmunityCommand(_user, _enableImmunity, _guildId);
        }
    }
}
