using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.IO;
using System.Collections.Generic;
using EthernetFunctons.Balaboba;
using EthernetFunctons;
using Constants;
using DiscordBotDURAK.EthernetFunctions;

namespace DiscordBotDURAK
{
    class Program
    {
        DiscordSocketClient client;
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();
        private static bool checkGuilds = true;

        private async Task MainAsync()
        {
            client = new DiscordSocketClient();
            client.MessageReceived += CommandsHandler;
            client.Log += Log;

            string token = null;

            using (StreamReader reader = new StreamReader(File.OpenRead(Constants.Constants.tokenPath)))
            {
                token = reader.ReadToEnd();
            }

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            await client.SetGameAsync("$help");

            Console.ReadLine();
        }

        public static Task Log(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }

        private Task CommandsHandler(SocketMessage message)
        {
            if (checkGuilds)
            {
                DeleteUnavalibleGuildsFDB(client.Guilds);
            }
            if (!DataBase.SearchChannelInDB(message.Channel.Id) && message.Channel.GetType().ToString() != "Discord.WebSocket.SocketDMChannel")
            {
                if (DataBase.SearchGuildIDB(((SocketGuildChannel)message.Channel).Guild.Id))
                {
                    DataBase.AddChannelInDB(((SocketGuildChannel)message.Channel).Guild.Id, message.Channel.Id);
                    message.Channel.SendMessageAsync("Этот канал помечен типом \"флуд\", чтобы " +
                        "показать сводку типов, а также узнать, как изменить тип " +
                        "напишите $help");
                }
                else
                {
                    AddGuildInDB(message);
                }
            }
            if (CheckChannel(message))
            {
                if (message.MentionedUsers.Count != 0 || message.MentionedRoles.Count != 0)
                {
                    if (DataBase.GetReferenceChannel(((SocketGuildChannel)message.Channel).Guild.Id) != message.Channel.Id)
                    {
                        _ = DeleteMessageAsync(message, enableTimer: true);
                    }
                }

                _ = Task.Run(() =>
                  {
                      if (!message.Author.IsBot)
                      {
                          if (message.Content.StartsWith(Commands.surf))
                          {
                              GetSurf(message);
                          }

                          if (message.Content.ToLower().StartsWith(Commands.quote))
                          {
                              MortarQuote(message);
                          }

                          if (message.Content.StartsWith(Commands.search))
                          {
                              SearchMessages(message);
                          }

                          if (message.Content.StartsWith(Commands.help))
                          {
                              CommandsHelp(message);
                          }

                          if (message.Content.ToLower().StartsWith(Commands.random))
                          {
                              RAND_Func(message);
                          }

                          if (message.Content.ToLower().Contains(Commands.decide))
                          {
                              Desider(message);
                          }

                          if (message.Content == Commands.id)
                          {
                              SendId(message);
                          }

                          if (RandomMessages.TriggerCheck(message.Content.ToLower()))
                          {
                              SHITPOST_Func(message);
                          }

                          if (message.Content.Contains("http"))
                          {
                              RefModeration(message);
                          }

                          if (CheckAdmin(message))
                          {
                              if (message.Content.ToLower() == Commands.owner)
                              {
                                  OwnerHelp(message);
                              }

                              if (message.Content.StartsWith(Commands.setChannelType))
                              {
                                  SetChannelType(message);
                              }

                              if (message.Content.ToLower().StartsWith(Commands.spam))
                              {
                                  SPAM_Func(message);
                              }

                              if (message.Content.ToLower().StartsWith(Commands.delete))
                              {
                                  DeleteFunc(message);
                              }

                              if (message.Content.ToLower().StartsWith(Commands.moderate))
                              {
                                  Moderate(message);
                              }

                              if (message.Content.ToLower().Contains(Commands.clean))
                              {
                                  Clear(message, Commands.clean);
                              }

                              if (message.Content.StartsWith(Commands.giveAdmin))
                              {
                                  GiveAdmin(message);
                              }
                          }
                      }
                  });
            }
            return Task.CompletedTask;
        }

        #region internal

        private void DeleteUnavalibleGuildsFDB(IReadOnlyCollection<SocketGuild> guilds)
        {
            List<ulong> dbguilds = DataBase.GetAll();
            foreach (var onlineGuild in guilds)
            {
                foreach (var dbguild in dbguilds.ToArray())
                {
                    if (onlineGuild.Id == dbguild)
                    {
                        dbguilds.Remove(dbguild);
                    }
                }
            }
            DataBase.DeleteGuilds(dbguilds);
        }

        /// <summary>
        /// Check if bot can moderate this channel
        /// </summary>
        /// <param name="message">message</param>
        /// <returns>true if bot can moderate this channel</returns>
        private bool CheckChannel(SocketMessage message)
        {
            if (message.Channel.GetType().ToString() == "Discord.WebSocket.SocketDMChannel")
            {
                return true;
            }
            if (!CheckGuild(message))
            {
                AddGuildInDB(message);
            }
            return DataBase.GetChannelType(message.Channel.Id) != ChannelSeverity.None;
        }

        //Check guilds

        private void AddGuildInDB(SocketMessage message)
        {
            SocketGuild guild = ((SocketGuildChannel)message.Channel).Guild;
            DataBase.AddAdminIDB(guild.Id, guild.OwnerId);
            _ = Log(new(LogSeverity.Info, Sources.internal_function, $"Add guild \"{guild.Name}\" in DB"));
            var channels = guild.Channels;
            foreach (var channel in channels)
            {
                DataBase.AddChannelInDB(guild.Id, channel.Id);
                _ = Log(new(LogSeverity.Info, Sources.internal_function, $"Add channel \"{channel.Name}\" in DB"));
            }
            Hello(guild.Owner);
        }

        private async void Hello(SocketGuildUser owner)
        {
            await owner.SendMessageAsync("Привет, я бот-дурак, ты владелец сервера, давай дружить!");
            await owner.SendMessageAsync("Сейчас ты - единственный и неповторимы админ для меня на этом сервере) " +
                "нужно будет добавить еще админов(если хочешь) и обязательно настроить чаты! Когда захочешь это сделать " +
                "напиши мне команду $owner (будет много букв, но я верю в тебя)");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns>true if guild was found in DB</returns>
        private bool CheckGuild(SocketMessage message)
        {
            ulong guildId = ((SocketGuildChannel)message.Channel).Guild.Id;
            return DataBase.SearchGuildIDB(guildId);
        }

        /// <summary>
        /// Sets a channel type
        /// </summary>
        /// <param name="message">message</param>
        private void SetChannelType(SocketMessage message)
        {
            int severity = 0;
            string msg = message.Content.Substring(Commands.setChannelType.Length).Trim().TrimEnd();
            try
            {
                severity = Convert.ToInt32(msg.Split(' ')[1]);
            }
            catch (IndexOutOfRangeException)
            {
                message.Channel.SendMessageAsync("Неправильная команда");
                return;
            }
            
            string mention = msg.Split(' ')[0];
            ulong id;
            if (mention.StartsWith("<#") && mention.EndsWith(">"))
            {
                id = Convert.ToUInt64(mention.Remove(0,2).Remove(18));
            }
            else
            {
                message.Channel.SendMessageAsync("Неправильно упомянут канал, канал упоминается в формате # + название канала");
                return;
            }
            if (Enum.IsDefined(typeof(ChannelSeverity), severity))
            {
                DataBase.AddChannelType(id, (ChannelSeverity)severity);
            }
            else
            {
                message.Channel.SendMessageAsync("Неправильно указан тип канала");
            }
        }
        
        /// <summary>
        /// Check if message author is admin
        /// </summary>
        /// <param name="message">message</param>
        /// <returns>true if message authos is admin</returns>
        private bool CheckAdmin(SocketMessage message)
        {
            if (message.Channel.GetType().ToString() == "Discord.WebSocket.SocketDMChannel")
            {
                return false;
            }
            SocketGuildChannel channel = (SocketGuildChannel)message.Channel;
            IEnumerable<string> adminCollection = DataBase.GetAdminsFDB(channel.Guild.Id);

            if (adminCollection == null)
            {
                return false;
            }
            else
            {
                foreach (var admin in adminCollection)
                {
                    if (Convert.ToUInt64(admin) == message.Author.Id)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        /// <summary>
        /// Deletes message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="enableTimer">true enables timer</param>
        /// <param name="timer">value of timer </param>
        /// <returns></returns>
        private async Task DeleteMessageAsync(SocketMessage message, bool enableTimer, int timer = 30000)
        {
            if (enableTimer)
            {
                if (DataBase.GetChannelType(message.Channel.Id) != ChannelSeverity.None || DataBase.GetChannelType(message.Channel.Id) != ChannelSeverity.References)
                {
                    await Task.Delay(timer);
                    await message.Channel.DeleteMessageAsync(message.Id);
                }
            }
            else
            {
                await message.Channel.DeleteMessageAsync(message.Id);
            }
            await Log(new(LogSeverity.Info, Sources.internal_function, "Message deleted"));
        }

        #endregion

        #region functions

        private async void GetSurf(SocketMessage message)
        {
            await message.Channel.SendMessageAsync(CSGOServers.GetAddress());
            await Log(new LogMessage(LogSeverity.Info, Sources.command, "Address sent"));
        }

        private async void MortarQuote(SocketMessage message)
        {
            var quoteChannel = client.GetChannel(546419059231555650);
            IAsyncEnumerable<IReadOnlyCollection<IMessage>> quotes = ((ISocketMessageChannel)quoteChannel).GetMessagesAsync(1000);
            Random random = new();
            List<IMessage> messages = new();
            await foreach (var quote in quotes)
            {
                foreach (var q in quote)
                {
                    messages.Add(q);
                }
            }
            string _quote =  messages.ToArray()[random.Next(messages.Count - 1)].Content;
            await message.Channel.SendMessageAsync(_quote);// работает, и славньо
            await Log(new LogMessage(LogSeverity.Info, Sources.command, $"Отправлена цитата в канал {message.Channel.Name}"));
        }

        private async void OwnerHelp(SocketMessage message)
        {
            string msg = new StreamReader(File.OpenRead(Constants.Constants.ownerMessagePath)).ReadToEnd();
            await message.Author.SendMessageAsync(msg);
        }

        private async void SearchMessages(SocketMessage message)
        {
            string content = message.Content.Remove(0, Commands.search.Length).Trim().TrimEnd();
            ISocketMessageChannel channel = message.Channel;
            IAsyncEnumerable<IReadOnlyCollection<IMessage>> EnumMessages = channel.GetMessagesAsync(1000);
            List<IMessage> findMessages = new();
            await foreach (var messages in EnumMessages)
            {
                foreach (var msg in messages)
                {
                    if (msg.Content.Contains(content))
                    {
                        findMessages.Add(msg);
                    }
                }
            }
            foreach (var msg in findMessages)
            {
                await channel.SendMessageAsync(
                    $"Автор: {msg.Author.Username}\n" +
                    $"Отправлено {msg.CreatedAt.DateTime}\n" +
                    $"Содержание: {msg.Content}");
            }
        }

        private void GiveAdmin(SocketMessage message)
        {
            if (message.MentionedEveryone)
            {
                return;
            }
            string admins = " ";
            ulong adminId;
            ulong guildId = ((SocketGuildChannel)message.Channel).Guild.Id;
            foreach (var user in message.MentionedUsers)
            {
                adminId = user.Id;
                DataBase.AddAdminIDB(guildId, adminId);
                admins = admins.Insert(admins.Length, user.Username + " ");
            }
            _ = Log(new(LogSeverity.Info, Sources.internal_function, $"Gave admin to{admins}"));
        }

        private void CommandsHelp(SocketMessage message)
        {
            message.Channel.SendMessageAsync(Commands.commandsHelp);
        }

        private async void RAND_Func(SocketMessage message)
        {
            await message.Channel.SendMessageAsync(
                $"{message.Author.Username}, Артему сегодня повезло, выпало число {new Random().Next(-1000, 1000)}");
            await Log(new(LogSeverity.Info, Sources.command, "Random has been randomed"));
        }

        private async void Desider(SocketMessage message)
        {
            if (new Random().Next(2) % 2 == 0)
            {
                await message.Channel.SendMessageAsync($"{message.Author.Username}, Да");
            }
            else
            {
                await message.Channel.SendMessageAsync($"{message.Author.Username}, Нет");
            }
            await Log(new(LogSeverity.Info, Sources.command, "Bot desided"));
        }

        private async void SPAM_Func(SocketMessage message)
        {
            if (message.Channel.Id == DataBase.GetReferenceChannel(((SocketGuildChannel)message.Channel).Guild.Id))
            {
                _ = message.Channel.SendMessageAsync("отказано");
                return;
            }
            if (message.Content.ToLower().Contains("http"))
            {
                return;
            }
            string[] msg = message.Content.Split(' ');
            int counter = 3;
            try
            {
                counter = Convert.ToInt32(msg[1]);
            }
            catch (FormatException) { }
            catch (IndexOutOfRangeException) { }
            string word = "";
            for (int i = 2; i < msg.Length; i++)
            {
                word += msg[i];
                word += " ";
            }
            try
            {
                for (int i = 0; i < counter; i++)
                {
                    await message.Channel.SendMessageAsync(word);
                }
            }
            catch (ArgumentException)
            {
                await message.Channel.SendMessageAsync("Нихрена не понимаю ни одного слова");
            }
            await Log(new(LogSeverity.Info, Sources.command, $"{word} spammed {counter} times"));
            //Clear(message, msg[0]+msg[1]);
        }

        private async void SHITPOST_Func(SocketMessage message)
        {
            await message.Channel.SendMessageAsync(
                $"{message.Author.Username}, {Balaboba.Say(message.Content)}");
        }

        private async void DeleteFunc(SocketMessage message)
        {
            int counter = Convert.ToInt32(message.Content.Substring(Commands.delete.Length).Trim().TrimEnd());
            var messages = message.Channel.GetMessagesAsync(counter);
            await foreach (var msg in messages)
            {
                foreach (var message1 in msg)
                {
                    await message1.DeleteAsync();
                }
            }
        }

        private async void RefModeration(IMessage message)
        {
            ulong referenceChannelId = DataBase.GetReferenceChannel(((SocketGuildChannel)message.Channel).Guild.Id);
            if (message.Channel.Id == referenceChannelId)
            {
                return;
            }
            if (message.Content.Contains("http"))
            {
                if (message.Content.Contains("gfycat"))
                {
                    await message.Channel.SendMessageAsync($"||{message.Content}||");
                    await message.DeleteAsync();
                    return;
                }
                if (message.Content.Contains("gif"))
                {
                    return;
                }
                ulong autorId = message.Author.Id;
                string content = message.Content;
                await message.DeleteAsync();
                ISocketMessageChannel channel = (ISocketMessageChannel)client.GetChannel(referenceChannelId);
                if (message.Content.ToLower().Contains("разд") || message.Content.ToLower().Contains("нитро") || message.Content.ToLower().Contains("nitro"))
                {
                    await channel.SendMessageAsync($"Вероятно, это очередной скам \n ||{content}||");

                    Console.WriteLine($"Переслано скам сообщение от {message.Author.Username}.");
                    return;
                }
                await channel.SendMessageAsync($"<@{autorId}>: \n\"{content}\"");
                await Log(new(LogSeverity.Info, Sources.command, $"Message from {message.Author.Username} has been redirected"));
            }
        }

        private async void SendId(SocketMessage message)
        {
            string id = Convert.ToString(message.Author.Id);
            await message.Author.SendMessageAsync($"Твой ID: {id}, id канала {message.Channel.Id}");
            await DeleteMessageAsync(message, enableTimer: false);
            await Log(new(LogSeverity.Info, Sources.command, $"ID send { message.Author.Username }"));
        }

        private async void Clear(SocketMessage message, string command = "")
        {
            string content = message.Content.Remove(0, command.Length).TrimStart().TrimEnd(); //сообщение без слова-команды
            var messages = message.Channel.GetMessagesAsync();
            await foreach (var item in messages)
            {
                foreach (var msg in item)
                {
                    if (msg.Content.Contains(content))
                    {
                        await msg.DeleteAsync();
                    }
                }
            }
            await Log(new(LogSeverity.Info, Sources.command, $"Cleaned channel {message.Channel.Name}"));
        }

        private async void Moderate(SocketMessage message)
        {
            var collection = message.Channel.GetMessagesAsync();
            await foreach (var messages in collection)
            {
                foreach (var msg in messages)
                {
                    RefModeration(msg);
                }
            }
            await message.DeleteAsync();
        }

        //private async void Moderate(List<SocketGuildChannel> channels)
        //{
        //    foreach (var channel in channels)
        //    {
        //        ITextChannel textChannel = (ITextChannel)channel;
        //        var messages = textChannel.GetMessagesAsync();
        //        await foreach (var message in messages)
        //        {
        //            foreach (var msg in message)
        //            {
        //                Moderate((SocketMessage)message);
        //            }
        //        }
        //    }
        //}

        #endregion

    }
}
