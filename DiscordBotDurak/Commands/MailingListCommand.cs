using DatabaseModel;
using Discord;
using Discord.WebSocket;
using DiscordBotDurak.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotDurak.Commands
{
    internal class MailingListCommand : ICommand
    {
        private enum ExitCode
        {
            Success,
            Added,
            UpToDate,
            Error
        }

        private readonly SocketGuildUser _user;
        private readonly MailingAction _action;

        public MailingListCommand(SocketGuildUser user, MailingAction action)
        {
            _user = user;
            _action = action;
        }

        private ExitCode ProcessUser(ulong userId, ulong guildId, bool mailing)
        {
            using var db = new Database();
            var user = db.GetUser(guildId, userId);
            ExitCode exitCode = ExitCode.Success;
            if (user is null)
            {
                db.BeginTransaction();
                user = new GuildUser()
                {
                    UserId = userId,
                    GuildId = guildId
                };
                db.AddUser(user);
                db.CommitAsync().Wait();
                exitCode = ExitCode.Added;
            }
            else if (user.Mailing == mailing)
                return ExitCode.UpToDate;
            try
            {
                db.BeginTransaction();
                db.SetMailing(guildId, userId, mailing);
                db.CommitAsync().Wait();
            }
            catch (NullReferenceException)
            {
                return ExitCode.Error;
            }
            return exitCode;
        }

        private StringBuilder GetAllMailingUsers(ulong guildId)
        {
            using var db = new Database();
            var users = db.GetAllMailing(guildId);
            var result = new StringBuilder();
            users.ForEach(u => result.AppendLine(_user.Guild.GetUser(u.UserId).DisplayName));
            return result;
        }

        public CommandResult GetResult()
        {
            var embed = new EmbedBuilder()
                .WithColor(Color.Blue)
                .WithFooter("mailing command")
                .WithCurrentTimestamp();
            if (_action == MailingAction.GetAll)
            {
                embed.AddField("Users:", GetAllMailingUsers(_user.Guild.Id).ToString());
                return new CommandResult().WithEmbed(embed);
            }
            bool mailing = _action == MailingAction.Add;
            var code = ProcessUser(_user.Id, _user.Guild.Id, mailing);
            if (code == ExitCode.Error)
                return new CommandResult().WithException("Something went wrong.");
            string status = mailing ? "enabled" : "disabled";
            if (code == ExitCode.Added)
                embed.AddField("Warning", $"User {_user.DisplayName} ({_user.Mention}) was added to database with default settings.");
            embed.AddField("Successfully:", $"Notifications for user {_user.DisplayName} {status}");
            return new CommandResult().WithEmbed(embed);
        }
    }
}
