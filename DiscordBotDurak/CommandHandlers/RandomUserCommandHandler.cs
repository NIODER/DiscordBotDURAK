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
            var mentionString = (string)slashCommand.Data.Options.First(option => option.Name == "mentions").Value;
            count = (long)(slashCommand.Data.Options.FirstOrDefault(options => options.Name == "count")?.Value ?? 1);
            guild = ((IGuildChannel)slashCommand.Channel).Guild;
            users = ProcessMentionsAsync(mentionString.Split(',')).Result;
            if (users.Count == 0)
                exception = new SlashCommandPropertyException("mentions", slashCommand.CommandName, "no mentions found");
        }

        private async IAsyncEnumerable<ulong> GetChannelUsersAsync(ulong channelId)
        {
            var channel = await guild.GetChannelAsync(channelId);
            await foreach (var _users in channel.GetUsersAsync())
            {
                if (_users.Any(u => u.VoiceChannel?.Id == channelId))
                    foreach (var user in _users.Where(u => u.VoiceChannel?.Id == channelId))
                        yield return user.Id;
                else
                    foreach (var user in _users)
                        yield return user.Id;
            }
        }

        private async Task<List<ulong>> ProcessMentionsAsync(IEnumerable<string> mentions)
        {
            var mentionsSeparated = mentions.Select(m => Utilities.FromMention(m.Trim()));
            var users = new List<ulong>();
            foreach (var (innerId, type) in mentionsSeparated)
            {
                switch (type)
                {
                    case MentionType.User:
                        users.Add(innerId);
                        break;
                    case MentionType.Channel:
                        await GetChannelUsersAsync(innerId).ForEachAsync(users.Add);
                        break;
                    case MentionType.Role:
                        users.AddRange(guild
                            .GetUsersAsync().Result
                            .Where(user => user.RoleIds.Contains(guild.GetRole(innerId).Id))
                            .Select(user => user.Id));
                        break;
                    case MentionType.Everyone:
                        users.AddRange(guild.GetUsersAsync().Result.Select(user => user.Id));
                        break;
                    case MentionType.Here:
                        users.AddRange(guild.GetUsersAsync().Result
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
