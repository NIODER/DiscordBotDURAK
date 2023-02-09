using DatabaseModel;
using Discord.WebSocket;
using DiscordBotDurak.Commands;
using DiscordBotDurak.Data;
using DiscordBotDurak.Enum.BotRoles;
using DiscordBotDurak.Exceptions;
using System.Linq;

namespace DiscordBotDurak.CommandHandlers
{
    internal class SetPrivelegeSlashCommandHandler : IVerifiable, ICommandHandler
    {
        private const string commandName = "Set privelege command";
        private readonly BotRole privelege;
        private readonly SocketGuildUser user;
        private readonly SocketUser author;

        public SetPrivelegeSlashCommandHandler(SocketSlashCommand command)
        {
            author = command.User;
            privelege = (BotRole)(long)command.Data.Options.First(op => op.Name == "privelege").Value;
            user = (SocketGuildUser)command.Data.Options.First(op => op.Name == "user").Value;
        }

        private bool ValidateRoleHierarchy(GuildUser dbUser, GuildUser dbCommandAuthor)
        {
            _ = Logger.Instance().LogAsync(new Discord.LogMessage(Discord.LogSeverity.Info,
                commandName,
                $"commandExcecutor: {dbCommandAuthor.Role}, user: {dbUser.Role} ({(BotRole)dbUser.Role})"));
            return dbCommandAuthor.Role > dbUser.Role;
        }

        private bool ValidateRoleAccess(GuildUser dbCommandAuthor)
        {
            _ = Logger.Instance().LogAsync(new Discord.LogMessage(Discord.LogSeverity.Info,
                commandName,
                $"executor role: {dbCommandAuthor.Role}, privelege: {(short)privelege} ({privelege})"));
            return dbCommandAuthor.Role >= (short)privelege;
        }

        public ICommand CreateCommand()
        {
            if (author is not SocketGuildUser gAuthor)
                return new SetPrivelegeCommand(
                    new DiscordBotSlashCommandException(
                        commandName, 
                        "This command can't be executed in direct message channel."));
            using var db = new Database();
            var botCommandExecutor = db.GetUser(gAuthor.Guild.Id, gAuthor.Id);
            if (!ValidateRoleAccess(botCommandExecutor))
                return new SetPrivelegeCommand(
                    new DiscordBotSlashCommandException(
                        commandName,
                        "You can't set role upwards yours."));
            var botUser = db.GetUser(user.Guild.Id, user.Id);
            if (!ValidateRoleHierarchy(botUser, botCommandExecutor))
                return new SetPrivelegeCommand(
                    new DiscordBotSlashCommandException(
                        commandName,
                        "You cannot reassign a role from a senior member."));
            return new SetPrivelegeCommand(privelege, botUser, user.Mention);
        }

        public bool Verify(ulong userId, ulong guildId)
        {
            using var db = new Database();
            var user = db.GetUser(guildId, userId);
            return user.Role >= (short)BotRole.Admin;
        }
    }
}
