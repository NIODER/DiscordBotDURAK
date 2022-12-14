using Discord.WebSocket;
using DiscordBotDurak.Enum.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBotDurak.Commands
{
    public class DeleteCommand : LastingCommand, ICommand
    {
        private readonly ISocketMessageChannel channel;
        private readonly string content;
        private readonly bool isByAuthor;

        public DeleteCommand(long type,
            ISocketMessageChannel channel,
            string content,
            ulong userId,
            ulong guildId,
            CommandType commandType) : base(userId, guildId, commandType)
        {
            isByAuthor = type == 1;
            this.channel = channel;
            this.content = content;
        }
        
        private async Task DeleteMessages()
        {
            var mentions = new List<ulong>();
            if (isByAuthor)
            {
                foreach (var mention in content.Split(','))
                {
                    var processedMention = Utilities.FromMention(mention.Trim());
                    switch (processedMention.type)
                    {
                        case Mentions.MentionType.User:
                            mentions.Add(processedMention.innerId);
                            break;
                        case Mentions.MentionType.Role:
                            await foreach (var users in channel.GetUsersAsync())
                            {
                                foreach (var user in users)
                                {
                                    if (user is SocketGuildUser guildUser)
                                    {
                                        var selectedUsers = guildUser.Roles
                                            ?.Where(r => r.Id == processedMention.innerId)
                                            ?.Select(u => u.Id);
                                        if (!(selectedUsers is null))
                                            mentions.AddRange(selectedUsers);
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            await foreach (var messages in channel.GetMessagesAsync())
            {
                foreach (var message in messages)
                {
                    if (CancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                    else
                    {
                        if (isByAuthor)
                        {
                            if (mentions.Contains(message.Author.Id))
                                await message.DeleteAsync();
                        }
                        else
                        {
                            if (message.Content.Contains(content))
                                await message.DeleteAsync();
                        }
                    }
                }
            }
        }

        public CommandResult GetResult()
        {
            _ = Task.Run(DeleteMessages, CancellationToken);
            return new CommandResult().WithText("Delete messages.");
        }
    }
}
