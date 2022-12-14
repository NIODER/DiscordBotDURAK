using Discord.WebSocket;
using DiscordBotDurak.Commands;
using DiscordBotDurak.Enum.ModerationModes;
using System.Linq;

namespace DiscordBotDurak.CommandHandlers
{
    internal class SetSpyModeSlashCommandHandler : ICommandHandler
    {
        private readonly SpyModesEnum _spyMode;
        private readonly SocketGuild _socketGuild;
        private readonly bool _isSetMode;

        public SetSpyModeSlashCommandHandler(SocketSlashCommand command)
        {
            _socketGuild = (command.Channel as SocketGuildChannel).Guild;
            var mode = command.Data.Options.FirstOrDefault(op => op.Name == "mode")?.Value;
            _isSetMode = mode is not null;
            if (_isSetMode)
            {
                _spyMode = (SpyModesEnum)(long)mode;
            }
        }

        public ICommand CreateCommand()
        {
            return new SetSpyModeCommand(_spyMode, _socketGuild, _isSetMode);
        }
    }
}
