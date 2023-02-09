using Discord;
using Discord.WebSocket;
using DiscordBotDurak.Commands;
using DiscordBotDurak.Exceptions;
using DiscordBotDurak.Mentions;
using DiscordBotDurak.Verification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBotDurak.CommandHandlers
{
    internal class RandomDistributionCommandHandler : AvailableToEveryone, ICommandHandler
    {
        private readonly List<ulong> users;
        private readonly long teamsCount;
        private readonly long teamSize;
        private readonly Exception exception;
        private readonly List<IGuildUser> guildUsers;

        public RandomDistributionCommandHandler(SocketSlashCommand command)
        {
            exception = null;
            if (command.Channel is IGuildChannel)
            {
                var mentionsString = (string)command.Data.Options.Where(option => option.Name == "mentions").First().Value;
                teamsCount = (long)command.Data.Options.Where(option => option.Name == "teams-count").First().Value;
                var teamsSizeOption = command.Data.Options.Where(option => option.Name == "team-size");
                if (teamsSizeOption.Count() != 0)
                {
                    teamSize = (long)teamsSizeOption.First().Value;
                }
                else
                {
                    teamSize = 0;
                }
                users = (List<ulong>)ProcessMentionsAsync(mentionsString.Split(','), command.Channel as IGuildChannel).Result;
                if (users.Count < teamSize * teamsCount || users.Count < teamSize || users.Count < teamsCount)
                {
                    teamSize = teamSize == 0 ? 1 : teamSize;
                    teamsCount = teamsCount == 0 ? 1 : teamsCount;
                    exception = new DiscordBotSlashCommandException(
                        "Random distribution command", 
                        $"number of users does not match,\n" +
                        $"users: {users.Count}\n" +
                        $"required: {teamsCount} (teams count) * {(teamSize == 0 ? 1 : teamSize)} (team size) = {teamsCount * teamSize}");
                }
                else
                {
                    guildUsers = new List<IGuildUser>();
                    foreach (var user in users)
                    {
                        guildUsers.Add((command.Channel as IGuildChannel).GetUserAsync(user).Result);
                    }
                }
            }
            else
            {
                exception = new DiscordBotSlashCommandException("random distribution command", "you can execute this command only in guild channel.");
            }
        }

        private async Task<IEnumerable<ulong>> ProcessMentionsAsync(IEnumerable<string> mentions, IGuildChannel guildChannel)
        {
            var mentionsSeparated = new List<(ulong id, MentionType type)>();
            foreach (var mention in mentions)
            {
                try
                {
                    mentionsSeparated.Add(Utilities.FromMention(mention.Trim()));
                }
                catch (ArgumentOutOfRangeException)
                {
                    continue;
                }
            }
            var users = new List<ulong>();
            foreach (var mention in mentionsSeparated)
            {
                switch (mention.type)
                {
                    case MentionType.User:
                        users.Add(mention.id);
                        break;
                    case MentionType.Channel:
                        await foreach (var _users in guildChannel.Guild.GetChannelAsync(mention.id).Result.GetUsersAsync())
                        {
                            foreach (var user in _users)
                            {
                                users.Add(user.Id);
                            }
                        }
                        break;
                    case MentionType.Role:
                        users.AddRange(guildChannel
                            .Guild.GetUsersAsync().Result
                            .Where(user => user.RoleIds.Contains(guildChannel.Guild.GetRole(mention.id).Id))
                            .Select(user => user.Id));
                        break;
                    case MentionType.Everyone:
                        users.AddRange(guildChannel.Guild.GetUsersAsync().Result.Select(user => user.Id));
                        break;
                    case MentionType.Here:
                        users.AddRange(guildChannel.Guild.GetUsersAsync().Result
                            .Where(user => user.Status == UserStatus.Online)
                            .Select(user => user.Id));
                        break;
                    default:
                        break;
                }
            }
            return users;
        }

        public ICommand CreateCommand() => new RandomDistributeCommand(guildUsers, teamsCount, teamSize, exception);
    }
}
