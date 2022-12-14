using DatabaseModel;
using Discord;
using DiscordBotDurak.Data;
using DiscordBotDurak.Enum.BotRoles;
using System;

namespace DiscordBotDurak.Commands
{
    public class SetPrivelegeCommand : ICommand
    {
        private readonly BotRole privelege;
        private readonly GuildUser user;
        private readonly string mention;
        private Exception exception;

        public SetPrivelegeCommand(BotRole privelege, GuildUser user, string mention)
        {
            this.privelege = privelege;
            this.user = user;
            this.mention = mention;
        }

        public SetPrivelegeCommand(Exception exception)
        {
            this.exception = exception;
        }

        private Exception SetPrivelege()
        {
            using var db = new Database();
            db.BeginTransaction();
            if (user is null)
            {
                var updatedUser = new GuildUser()
                {
                    UserId = user.UserId,
                    GuildId = user.GuildId,
                    Role = (short)privelege
                };
                db.AddUser(updatedUser);
            }
            else
            {
                user.Role = (short)privelege;
                db.UpdateUser(user);
            }
            db.CommitSychronized();
            return db.Exception;
        }

        public CommandResult GetResult()
        {
            if (!(exception is null))
            {
                _ = Logger.Instance().LogAsync(new LogMessage(LogSeverity.Error, "Set privelege command", "Error occured.", exception));
                return new CommandResult()
                    .WithException(exception);
            }
            exception = SetPrivelege();
            if (!(exception is null))
            {
                _ = Logger.Instance().LogAsync(new LogMessage(LogSeverity.Error,
                    "SetPrivelegeCommand",
                    "Exception occured while update user.",
                    exception));
                return new CommandResult().WithEmbed(new EmbedBuilder()
                    .WithColor(Color.Red)
                    .AddField("Exception occured", "Changes declined.")
                    .WithFooter("Set privelege command")
                    .WithCurrentTimestamp());
            }
            return new CommandResult().WithEmbed(new EmbedBuilder()
                    .WithColor(Color.Blue)
                    .AddField("User updated",
                    $"User: {mention},\n" +
                    $"Role: {privelege}")
                    .WithFooter("Set privelege command")
                    .WithCurrentTimestamp());
        }
    }
}
