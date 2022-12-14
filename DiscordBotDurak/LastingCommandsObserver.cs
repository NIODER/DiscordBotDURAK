using Discord;
using DiscordBotDurak.Enum.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscordBotDurak
{
    public class LastingCommandsObserver
    {
        private static LastingCommandsObserver instance;
        private List<LastingCommand> lastingCommands;

        private LastingCommandsObserver()
        {
            lastingCommands = new List<LastingCommand>();
        }

        public static LastingCommandsObserver Instance()
        {
            if (instance is null)
            {
                instance = new LastingCommandsObserver();
            }
            return instance;
        }

        public void Add(LastingCommand lastingCommand)
        {
            lastingCommands.Add(lastingCommand);
        }

        public bool Remove(LastingCommand lastingCommand)
        {
            return lastingCommands.Remove(lastingCommand);
        }

        public void Cancel(ulong authorId, ulong guildId, CommandType commandType)
        {
            var commands = (from c in lastingCommands
                           where c.AuthorId == authorId
                           where c.GuildId == guildId
                           where c.CommandType == commandType
                           select c).ToList();
            foreach (var command in commands)
                command.Interrupt();
        }

        public void Cancel(ulong guildId, CommandType commandType)
        {
            var commands = from c in lastingCommands
                           where c.GuildId == guildId || c.CommandType == commandType
                           select c;
            try
            {
                foreach (var command in commands)
                    command.Interrupt();
            }
            catch (Exception e)
            {
                _ = Logger.Instance().LogAsync(
                    new LogMessage(LogSeverity.Error, 
                    "LastingCommandsObserver", 
                    "In result of thread race command finished faster, than stopped.", e));
            }
        }
    }
}
