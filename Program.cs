using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.IO;
using System.Collections.Generic;
using EthernetFunctons.Balaboba;
using Constants;

namespace DiscordBotDURAK
{
    class Program
    {
        DiscordSocketClient client;
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

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

        private Task Log(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }

        private Task CommandsHandler(SocketMessage message)
        {
            if ((message.MentionedUsers.Count != 0 || message.MentionedRoles.Count != 0) && message.Author.Id != 881591057953992724)
                _ = DeleteMessageAsync(message, enableTimer: true);

            _ = Task.Run(() =>
              {
                  if (!message.Author.IsBot)
                  {
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
                          if (message.Content.ToLower().StartsWith(Commands.spam))
                          {
                              SPAM_Func(message);
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
                  CheckGuilds(client.Guilds);
              });
            return Task.CompletedTask;
        }

        #region common functions

        /// <summary>
        /// Add new guild in DB if its wasn't already there
        /// </summary>
        /// <param name="guilds">Collection of all bot's guilds</param>
        private void CheckGuilds(IReadOnlyCollection<SocketGuild> guilds)
        {
            foreach (var guild in guilds)
            {
                if (!DataBase.Search(guild.Id))
                {
                    DataBase.Add(guild.Id, guild.OwnerId);
                    _ = Log(new(LogSeverity.Info, Sources.internal_function, $"Add guild \"{guild.Name}\" in DB"));
                }
            }
        }

        /// <summary>
        /// Check if message author is admin
        /// </summary>
        /// <param name="message">message</param>
        /// <returns>true if message authos is admin</returns>
        private bool CheckAdmin(SocketMessage message)
        {
            SocketGuildChannel channel = (SocketGuildChannel)message.Channel;
            IEnumerable<string> adminCollection = DataBase.Get(channel.Guild.Id);

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

        private async Task DeleteMessageAsync(SocketMessage message, bool enableTimer, int timer = 30000)
        {
            if (enableTimer)
            {
                if (message.Channel.Id != 720193176463343666)
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

        #region BOT FUNCTIONS

        private void GiveAdmin(SocketMessage message)
        {
            if (message.MentionedEveryone)
            {
                return;
            }
            string admins = null;
            ulong adminId;
            ulong guildId = ((SocketGuildChannel)message.Channel).Guild.Id;
            foreach (var user in message.MentionedUsers)
            {
                adminId = user.Id;
                DataBase.Add(guildId, adminId);
                admins = admins.Insert(admins.Length, user.Username + " ");
            }
            _ = Log(new(LogSeverity.Info, Sources.internal_function, $"Gave admin to {admins}"));
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
            if (message.Channel.Id == 720193176463343666)
            {
                _ = message.Channel.SendMessageAsync("отказано");
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

        private async void RefModeration(IMessage message)
        {
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
                ISocketMessageChannel channel = (ISocketMessageChannel)client.GetChannel(720193176463343666);
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
            await message.Author.SendMessageAsync($"Твой ID: {id}");
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
