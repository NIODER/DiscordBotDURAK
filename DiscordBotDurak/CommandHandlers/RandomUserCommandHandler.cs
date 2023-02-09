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
    internal class RandomUserCommandHandler : AvailableToEveryone, ICommandHandler
    {
        private readonly List<ulong> users;
        private readonly long count;
        private readonly Exception exception;
        private readonly IGuild guild;

        public RandomUserCommandHandler(SocketSlashCommand slashCommand)
        {
            users = new List<ulong>();
            exception = null;
            var mentionString = (string)slashCommand.Data.Options.Where(option => option.Name == "mentions").First().Value;
            var countOption = slashCommand.Data.Options.Where(options => options.Name == "count");
            count = countOption.Count() < 1 ? 1 : (long)countOption.First().Value; // если не указали count, то он 1
            if (slashCommand.Channel is IGuildChannel)
            {
                users = (List<ulong>)ProcessMentionsAsync(mentionString.Split(','), slashCommand.Channel as IGuildChannel).Result;
                guild = (slashCommand.Channel as IGuildChannel).Guild;
            }
            if (users.Count == 0)
                exception = new SlashCommandPropertyException("mentions", slashCommand.CommandName, "no mentions found");
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

        public ICommand CreateCommand()
        {
            return new RandomUserCommand(users, count, guild, exception);
        }
    }
}
