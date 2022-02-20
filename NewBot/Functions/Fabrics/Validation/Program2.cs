using Discord;
using Discord.WebSocket;
using DiscordBotDURAK.NewBot.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotDURAK.NewBot.Functions.Fabrics.Validation
{
    class Program2
    {
        CRUD database = new Database();

        private Task CommandsHandler(SocketMessage message)
        {
            //CheckGuilds();
            bool checkChannel = (Data.ChannelSeverity)database.Read(Actions.Channel, true, Convert.ToString(message.GuildId())) != Data.ChannelSeverity.None;
            checkChannel = checkChannel || message.Channel.GetType().ToString() == "Discord.WebSocket.SocketDMChannel";
            if (!checkChannel)
            {
                return Task.CompletedTask;
            }

            if (message.MentionedUsers.Count != 0 || message.MentionedRoles.Count != 0)
            {
                if (message.Channel.Id != (ulong)database.Read(Actions.Channel, false, Convert.ToString(((SocketGuildChannel)message.Channel).Guild.Id)))
                {
                    _ = DeleteMessageAsync(message, true);
                }
            }

            if (message.Author.IsBot)
            {
                return Task.CompletedTask;
            }

            Validator validator = new(message.Content, database);
            IFabric fabric = validator.Fabric;
            IFunction function = fabric.GetFunction();
            string answer = (string)function.Execute(message.Content.Split(" "));


            return Task.CompletedTask;
        }

        private void CheckGuilds(IReadOnlyCollection<SocketGuild> guilds)
        {
            List<ulong> databaseGuilds = (List<ulong>)database.Read(Actions.Guild);
            bool delete;
            foreach (var dbguild in databaseGuilds)
            {
                delete = true;
                foreach (var guild in guilds)
                {
                    if (dbguild == guild.Id)
                    {
                        delete = false;
                        break;
                    }
                }
                if (delete)
                {
                    database.Delete(Actions.Guild, Convert.ToString(dbguild));
                }
            }
            bool add;
            databaseGuilds = (List<ulong>)database.Read(Actions.Guild);
            foreach (var guild in guilds)
            {
                add = true;
                foreach (var dbguild in databaseGuilds)
                {
                    if (dbguild == guild.Id)
                    {
                        add = false;
                        break;
                    }
                }
                if (add)
                {
                    string guildId = Convert.ToString(guild.Id);
                    database.Create(Actions.Guild, guildId);
                    database.Create(Actions.Admin, guildId, guild.OwnerId);
                    foreach (var channel in guild.Channels)
                    {
                        database.Create(Actions.Channel, guildId, channel.Id);
                    }
                    guild.Owner.SendMessageAsync($"Привет, я бот-дурак, ты владелец сервера, давай дружить!");
                    guild.Owner.SendMessageAsync("Сейчас ты - единственный и неповторимы админ для меня на этом сервере) " +
                        "нужно будет добавить еще админов(если хочешь) и обязательно настроить чаты! Когда захочешь это сделать " +
                        "напиши мне команду $owner (будет много букв, но я верю в тебя)");
                }
            }
        }

        private async Task DeleteMessageAsync(SocketMessage message, bool enableTimer, int timer = 30000)
        {
            if (enableTimer)
            {
                if (message.Channel.ChannelType() != ChannelSeverity.None || message.Channel.IsReferences())
                {
                    await Task.Delay(timer);
                    await message.Channel.DeleteMessageAsync(message.Id);
                }
            }
            else
            {
                await message.Channel.DeleteMessageAsync(message.Id);
            }
            new Log(new(LogSeverity.Info, "internal", "Message deleted"));
        }
    }
}
