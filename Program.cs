using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.IO;
using System.Collections.Generic;
using EthernetFunctons.Balaboba;
using Constants;
using DiscordBotDURAK.EthernetFunctions;
using CyberShoke;

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
            client.SelectMenuExecuted += SelectMenuHandler;
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

        public async Task SelectMenuHandler(SocketMessageComponent component)
        {
            switch (component.Data.CustomId)
            {
                case "cs-main":
                    var csMain = CybershokeCSGOCommandButtons.CsMain(component);
                    if (csMain.text is not null) csMain.builder = null;
                    _ = component.Channel.SendMessageAsync(csMain.text ?? "Режимы:", components: csMain.builder?.Build() ?? null).Result.DeleteMessageAsync(true, 60000);
                    break;
                case "DUELS2X2":
                    await component.Channel.SendMessageAsync(CybershokeCSGOCommandButtons.Duplets(component));
                    break;
                case "DUELS":
                    await component.Channel.SendMessageAsync(CybershokeCSGOCommandButtons.Duels(component));
                    break;
                case "RETAKE":
                    await component.Channel.SendMessageAsync(CybershokeCSGOCommandButtons.Retake(component));
                    break;
                case "RETAKECLASSIC":
                    await component.Channel.SendMessageAsync(CybershokeCSGOCommandButtons.RetakeClasiic(component));
                    break;
                case "DM":
                    await component.Channel.SendMessageAsync(CybershokeCSGOCommandButtons.DM(component));
                    break;
                case "PISTOLDM":
                    await component.Channel.SendMessageAsync(CybershokeCSGOCommandButtons.PISTOLDM(component));
                    break;
                case "AWPDM":
                    await component.Channel.SendMessageAsync(CybershokeCSGOCommandButtons.AWPDM(component));
                    break;
                case "AIMDM":
                    await component.Channel.SendMessageAsync(CybershokeCSGOCommandButtons.AIMDM(component));
                    break;
                case "SURF":
                    await component.Channel.SendMessageAsync(CybershokeCSGOCommandButtons.SURF(component));
                    break;
                case "BHOP":
                    await component.Channel.SendMessageAsync(CybershokeCSGOCommandButtons.BHOP(component));
                    break;
                case "KZ":
                    await component.Channel.SendMessageAsync(CybershokeCSGOCommandButtons.KZ(component));
                    break;
                case "PUBLIC":
                    await component.Channel.SendMessageAsync(CybershokeCSGOCommandButtons.PUBLIC(component));
                    break;
                case "AWP":
                    await component.Channel.SendMessageAsync(CybershokeCSGOCommandButtons.AWP(component));
                    break;
                case "HNS":
                    await component.Channel.SendMessageAsync(CybershokeCSGOCommandButtons.HNS(component));
                    break;
                default:
                    break;
            }
            await component.DeferAsync();
            _ = DeleteMessageAsync(component.Message, true, 60000);
        }

        private Task CommandsHandler(SocketMessage message)
        {
            _ = Task.Run(async () =>
            {
                CheckGuilds(client.Guilds);
                if (!message.Channel.isDirect() && message.Channel.ChannelType() == ChannelSeverity.NoSuchChannel)
                {
                    MyDatabase.AddChannel(message.GuildId(), message.Channel.Id);
                    await message.Channel.SendMessageAsync("Этот канал помечен типом \"флуд\", чтобы " +
                        "показать сводку типов, а также узнать, как изменить тип " +
                        "напишите $help");
                }
                if (!CheckChannel(message))
                {
                    return;
                }
                if (!isReply((IUserMessage)message) && (message.MentionedUsers.Count != 0 || message.MentionedRoles.Count != 0) && !message.Channel.IsReferences())
                {
                    await DeleteMessageAsync(message, enableTimer: true);
                }
                if (message.Author.IsBot)
                {
                    return;
                }

                if (message.Content.StartsWith(Commands.radio))
                {
                    GetRadio(message);
                }
                if (message.Content.StartsWith(Commands.joke))
                {
                    GetJoke(message);
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
                if (message.Content.StartsWith(Commands.cyberShoke))
                {
                    GetCSGOServer(message);
                }
                if (message.IsAuthorAdmin())
                {
                    if (message.Content.StartsWith(Commands.deleteFavorRadio))
                    {
                        DeleteFavour(message);
                    }
                    if (message.Content.StartsWith(Commands.setRadio))
                    {
                        SetFavour(message);
                    }
                    if (message.Content.StartsWith(Commands.joinedAt))
                    {
                        WhenJoined(message);
                    }
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
            );
            return Task.CompletedTask;
        }

        #region internal

        private bool isReply(IUserMessage message) => message?.ReferencedMessage?.Content is not null;

        private void CheckGuilds(IReadOnlyCollection<SocketGuild> guilds)
        {
            var dbguilds = MyDatabase.GetGuilds();
            bool delete;
            foreach (var dbguild in dbguilds)
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
                    MyDatabase.DeleteGuild(Convert.ToString(dbguild));
                }
            }
            bool add;
            dbguilds = MyDatabase.GetGuilds();
            foreach (var guild in guilds)
            {
                add = true;

                foreach (var dbguild in dbguilds)
                {
                    if (dbguild == guild.Id)
                    {
                        add = false;
                        break;
                    }
                }
                if (add)
                {
                    AddGuildInDB(guild);
                }
            }
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
                Console.WriteLine($"channel {message.Channel.Id} is dmc");
                return true;
            }
            if (!((SocketGuildChannel)message.Channel).Guild.Check())
            {
                Console.WriteLine($"add channel {message.Channel.Id}");
                AddGuildInDB(message);
            }
            return message.Channel.ChannelType() != ChannelSeverity.None;
        }

        private void AddGuildInDB(SocketMessage message)
        {
            SocketGuild guild = ((SocketGuildChannel)message.Channel).Guild;
            string guildId = message.GuildId();
            MyDatabase.AddGuild(guildId);
            MyDatabase.AddAdmin(guildId, guild.OwnerId);
            foreach (var channel in guild.Channels)
            {
                MyDatabase.AddChannel(guildId, channel.Id);
            }
            Hello(guild.Owner);
        }

        private void AddGuildInDB(SocketGuild guild)
        {
            string guildId = Convert.ToString(guild.Id);
            MyDatabase.AddGuild(guildId);
            MyDatabase.AddAdmin(guildId, guild.OwnerId);
            foreach (var channel in guild.Channels)
            {
                MyDatabase.AddChannel(guildId, channel.Id);
            }
            Hello(guild.Owner);
        }



        private async void Hello(SocketGuildUser owner)
        {
            await owner.SendMessageAsync($"Привет, я бот-дурак, ты владелец сервера, давай дружить!");
            await owner.SendMessageAsync("Сейчас ты - единственный и неповторимы админ для меня на этом сервере) " +
                "нужно будет добавить еще админов(если хочешь) и обязательно настроить чаты! Когда захочешь это сделать " +
                "напиши мне команду $owner (будет много букв, но я верю в тебя)");
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
                MyDatabase.UpdateChannelType(message.GuildId(), id, (ChannelSeverity)severity);
            }
            else
            {
                message.Channel.SendMessageAsync("Неправильно указан тип канала");
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
            await Log(new(LogSeverity.Info, Sources.internal_function, "Message deleted"));
        }

        #endregion

        #region functions
        private async void GetCSGOServer(SocketMessage message)
        {
            SelectMenuBuilder selectMenuBuilder = new SelectMenuBuilder()
                .WithPlaceholder("Выберите режим")
                .WithCustomId("cs-main")
                .WithMinValues(1)
                .WithMaxValues(1);

            string banned = "AMONGUS MINIGAMES SURFCOMBAT JAIL DEATHRUN";
            var listmodes = new CSServers().Listmodes;

            foreach (var item in banned.Split(" "))
            {
                listmodes.Remove(item);
            }

            foreach (var item in listmodes)
            {
                selectMenuBuilder.AddOption(label: item, value: item);
            }

            var builder = new ComponentBuilder()
                .WithSelectMenu(selectMenuBuilder);

            await message.Channel.SendMessageAsync(text: "Доступно:", components: builder.Build());
        }

        private async void DeleteFavour(SocketMessage message)
        {
            string reference = message.Content.Split(' ')[1];
            MyDatabase.DeleteRadio(message.GuildId(), reference);
            await Log(new LogMessage(LogSeverity.Info, Sources.command, "Radio deleted"));
        }

        private async void GetFavour(SocketMessage message)
        {
            var refs = MyDatabase.GetRadio(message.GuildId());
            foreach (string reference in refs)
            {
                await message.Channel.SendMessageAsync(reference);
            }
        }

        private async void SetFavour(SocketMessage message)
        {
            string reference = message.Content.Split(' ')[1];
            MyDatabase.AddRadio(message.GuildId(), reference);
            await Log(new LogMessage(LogSeverity.Info, Sources.command, "Reference added"));
        }

        private async void GetRadio(SocketMessage message)
        {
            string key = message.Content.Remove(0, Commands.radio.Length).Trim();
            try
            {
                await message.Channel.SendMessageAsync(RadioReferences.GetRadio(key).Remove(2000));
            }
            catch (ArgumentException)
            {
                await message.Channel.SendMessageAsync("Я не нашел ничего подходящего");
                await Log(new LogMessage(LogSeverity.Error, Sources.command, "Radios not sent"));
                return;
            }
            await Log(new LogMessage(LogSeverity.Info, Sources.command, "Radios sent"));
        }

        private async void WhenJoined(SocketMessage message)
        {
            var users = message.MentionedUsers;
            foreach (var user in users)
            {
                var socketGuildUser = (SocketGuildUser)user;
                await message.Channel.SendMessageAsync($"{socketGuildUser.Username} присоеденился {socketGuildUser.JoinedAt}");
            }
        }

        private async void GetJoke(SocketMessage message)
        {
            string joke = Joke.GetJoke();
            await message.Channel.SendMessageAsync(joke);
            await Log(new LogMessage(LogSeverity.Info, Sources.command, $"GetJoke() : {joke}"));
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
            foreach (var user in message.MentionedUsers)
            {
                MyDatabase.AddAdmin(message.GuildId(), user.Id);
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
            if (message.Channel.IsReferences())
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
                await message.Channel.SendMessageAsync($"Правильно \"$spam 5 aboba\", к примеру");
            }
            await Log(new(LogSeverity.Info, Sources.command, $"{word} spammed {counter} times"));
        }

        private void SHITPOST_Func(SocketMessage message)
        {
            string authorUsername = message.Author.Username;
            string balabobaAnswer = Balaboba.Say(message.Content);

            if (message.Channel.IsReferences() || balabobaAnswer is null)
            {
                return;
            }
            else
            {
                _ = message.Channel.SendMessageAsync($"{authorUsername}, {balabobaAnswer}");
            }
        }

        private async void RefModeration(IMessage message)
        {
            ISocketMessageChannel channel = ((ISocketMessageChannel)message.Channel);
            if (channel.IsReferences())
            {
                return;
            }
            if (message.Content.Contains("gfycat") && message.Content.Contains("gif"))
            {
                await message.Channel.SendMessageAsync($"||{message.Content}||");
                await message.DeleteAsync();
                return;
            }
            ulong refsChannelId = MyDatabase.GetReferencesChannel(Convert.ToString(((SocketGuildChannel)channel).Guild.Id));
            if (refsChannelId == 0)
            {
                return;
            }
            ulong autorId = message.Author.Id;
            string content = message.Content;
            ISocketMessageChannel referencesChannel =
                client.GetChannel(refsChannelId) as ISocketMessageChannel;
            if (message.Content.ToLower().Contains("разд") || message.Content.ToLower().Contains("нитро") || message.Content.ToLower().Contains("nitro"))
            {
                await referencesChannel.SendMessageAsync($"Вероятно, это очередной скам \n ||{content}||");

                Console.WriteLine($"Переслано скам сообщение от {message.Author.Username}.");
                return;
            }
            if (message.Content.Contains("tenor.com"))
            {
                return;
            }
            await referencesChannel.SendMessageAsync($"<@{autorId}>: \n\"{content}\"");
            await message.DeleteAsync();
            await Log(new(LogSeverity.Info, Sources.command, $"Message from {message.Author.Username} has been redirected"));
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
            string content = message.Content.Remove(0, command.Length).TrimStart().TrimEnd();
            var messages = message.Channel.GetMessagesAsync();
            try
            {
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
            }
            catch (Discord.Net.HttpException e )
            {
                await Log(new LogMessage(LogSeverity.Error, "HttpException", e.Message));
            }
            await Log(new LogMessage(LogSeverity.Info, Sources.command, $"Cleaned channel {message.Channel.Name}"));
        }

        private async void Moderate(SocketMessage message)
        {
            if (message.Channel.IsReferences())
            {
                return;
            }
            var collection = message.Channel.GetMessagesAsync();
            await foreach (var messages in collection)
            {
                foreach (var msg in messages)
                {
                    if (msg.Content.Contains("<@") && msg.Content.Contains(">"))
                    {
                        await msg.DeleteAsync();
                        continue;
                    }
                }
            }
            await message.DeleteAsync();
            await Log(new LogMessage(LogSeverity.Info, Sources.command, $"Channel {message.Channel.Name} cleaned"));
        }
        #endregion
    }
}
