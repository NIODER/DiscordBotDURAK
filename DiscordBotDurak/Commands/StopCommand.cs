using DatabaseModel;
using DiscordBotDurak.Data;
using DiscordBotDurak.Enum.BotRoles;
using DiscordBotDurak.Enum.Commands;

namespace DiscordBotDurak.Commands
{
    class StopCommand : ICommand
    {
        private readonly CommandType commandType;
        private readonly ulong authorId;
        private readonly ulong guildId;
        private readonly LastingCommandsObserver commandsObserver;

        public StopCommand(CommandType commandType, ulong authorId, ulong guildId)
        {
            this.commandType = commandType;
            this.authorId = authorId;
            this.guildId = guildId;
            commandsObserver = LastingCommandsObserver.Instance();
        }

        public CommandResult GetResult()
        {
            CommandResult commandResult;
            using var db = new Database();
            var user = db.GetUser(guildId, authorId) ?? db.AddUser(new GuildUser()
            {
                UserId = authorId,
                GuildId = guildId
            });
            BotRole role = (BotRole)db.GetUser(guildId, authorId).Role;
            if (role >= BotRole.Moderator)
                commandsObserver.Cancel(guildId, commandType);
            else commandsObserver.Cancel(authorId, guildId, commandType);
            commandResult = new CommandResult().WithText("Stopped");
            return commandResult;
        }
    }
}
