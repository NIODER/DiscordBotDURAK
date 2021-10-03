using System;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using Discord.WebSocket;
using System.IO;
using System.Collections.Generic;
using EthernetFunctons.Balaboba;

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

            var token = "";

            using (StreamReader reader = new StreamReader(File.OpenRead(Constants.Constants.tokenPath)))
            {
                token = reader.ReadToEnd();
            }

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

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
                      
                      if (message.Content.StartsWith("RAND"))
                          RAND_Func(message);

                      if (message.Content.StartsWith("SPAM"))
                          SPAM_Func(message);

                      if (message.Content.ToLower().Contains("бот") || RandomMessages.TriggerCheck(message.Content.ToLower()))
                          SHITPOST_Func(message);

                      if (message.Content.ToLower().Contains("реши"))
                          Desider(message);

                      if (message.Content == "ID")
                          SendId(message);

                      if (message.Content.Contains("http"))
                          RefModeration(message);

                      if (message.Content.ToLower().Contains("чистин"))
                          Clear(message);
                  }
              });
            return Task.CompletedTask;
        }

        #region BOT FUNCTIONS

        private async void RAND_Func(SocketMessage message)
        {
            await message.Channel.SendMessageAsync(
                $"{message.Author.Username}, Артему сегодня повезло, выпало число {new Random().Next(-1000, 1000)}");
            Console.WriteLine($"{DateTime.Now}\nОтработал рандом\n__________________________________________\n");
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
            Console.WriteLine($"{DateTime.Now}\nБот принял решение\n__________________________________________\n");
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
            Console.WriteLine($"{DateTime.Now}\nПроспамлено {word} {counter} раз\n__________________________________________\n");
        }

        private async void SHITPOST_Func(SocketMessage message)
        {
            await message.Channel.SendMessageAsync(
                $"{message.Author.Username}, {Balaboba.Say(message.Content)}");
        }

        private async void RefModeration(SocketMessage message)
        {
            ulong autorId = message.Author.Id;
            string content = message.Content;
            await DeleteMessageAsync(message, enableTimer: false);
            ISocketMessageChannel channel = (ISocketMessageChannel)client.GetChannel(720193176463343666);
            if (message.Content.ToLower().Contains("разд") || message.Content.ToLower().Contains("нитро") || message.Content.ToLower().Contains("nitro"))
            {
                await channel.SendMessageAsync($"Вероятно, это очередной скам \n ||{content}||");
                
                Console.WriteLine($"Переслано скам сообщение от {message.Author.Username}.");
                return;
            }
            await channel.SendMessageAsync($"<@{autorId}>: \n\"{content}\"");
            Console.WriteLine($"{DateTime.Now}\nПереслано сообщение от {message.Author.Username}.\n__________________________________________\n");
        }

        private async void SendId(SocketMessage message)
        {
            string id = Convert.ToString(message.Author.Id);
            await message.Author.SendMessageAsync($"Твой ID: {id}");
            await DeleteMessageAsync(message, enableTimer: false);
            Console.WriteLine($"{DateTime.Now}\nОтправлено ID {message.Author.Username}.\n__________________________________________\n");
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
            Console.WriteLine($"{DateTime.Now}\nУдалено сообщение\n{message.Id}\n{message.Content}\n__________________________________________\n");
        }

        private async void Clear(SocketMessage message)
        {
            string content = message.Content.Remove(0, 6).TrimStart().TrimEnd();
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
            Console.WriteLine($"{DateTime.Now}\nЧистин отработал в канале {message.Channel.Name}\n__________________________________________\n");
        }

        #endregion

    }
}
