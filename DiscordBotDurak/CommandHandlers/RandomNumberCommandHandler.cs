using Discord.WebSocket;
using DiscordBotDurak.Commands;
using DiscordBotDurak.Exceptions;
using DiscordBotDurak.Verification;
using System;
using System.Linq;

namespace DiscordBotDurak.CommandHandlers
{
    internal class RandomNumberCommandHandler : AvailableToEveryone, ICommandHandler
    {
        private readonly long max;
        private readonly long min;
        private readonly long count;
        private readonly Exception exception;

        public RandomNumberCommandHandler(SocketSlashCommand command)
        {
            max = (long)command.Data.Options.Where(option => option.Name == "max").First().Value;
            min = (long)command.Data.Options.Where(option => option.Name == "min").First().Value;
            var countOption = command.Data.Options.Where(option => option.Name == "count");
            count = countOption.Count() < 1 ? 1 : (long)countOption.First().Value;
            exception = count > 100 ? new SlashCommandPropertyException(
                countOption.First().Name,
                command.CommandName,
                $"{countOption.First().Name} need be less than 100") : null;
        }

        public ICommand CreateCommand() => new RandomNumberCommand(min, max, count, exception);
    }
}
