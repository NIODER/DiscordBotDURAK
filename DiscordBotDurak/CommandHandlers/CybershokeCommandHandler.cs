using Discord.WebSocket;
using DiscordBotDurak.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscordBotDurak.CommandHandlers
{
    public class CybershokeCommandHandler : ICommandHandler
    {
        private enum CommandType
        {
            First,
            Second,
            Third
        }
        private readonly CommandType commandType;
        private readonly string customId;
        private readonly string value;
        private readonly Exception exception;

        public CybershokeCommandHandler()
        {
            commandType = CommandType.First;
        }

        public CybershokeCommandHandler(SocketMessageComponent component)
        {
            customId = component.Data.CustomId;
            value = component.Data.Values.First();
            switch (customId)
            {
                case "cs-main":
                    commandType = CommandType.Second;
                    break;
                case "DUELS2X2":
                    commandType = CommandType.Third;
                    break;
                case "DUELS":
                    commandType = CommandType.Third;
                    break;
                case "RETAKE":
                    commandType = CommandType.Third;
                    break;
                case "RETAKECLASSIC":
                    commandType = CommandType.Third;
                    break;
                case "DM":
                    commandType = CommandType.Third;
                    break;
                case "PISTOLDM":
                    commandType = CommandType.Third;
                    break;
                case "AWPDM":
                    commandType = CommandType.Third;
                    break;
                case "AIMDM":
                    commandType = CommandType.Third;
                    break;
                case "SURF":
                    commandType = CommandType.Third;
                    break;
                case "BHOP":
                    commandType = CommandType.Third;
                    break;
                case "KZ":
                    commandType = CommandType.Third;
                    break;
                case "PUBLIC":
                    commandType = CommandType.Third;
                    break;
                case "AWP":
                    commandType = CommandType.Third;
                    break;
                case "HNS":
                    commandType = CommandType.Third;
                    break;
                default:
                    exception = new NotImplementedException("Not implemented id");
                    break;
            }
        }

        public ICommand CreateCommand()
        {
            if (!(exception is null))
                return null;
            return commandType switch
            {
                CommandType.First => new CybershokeCommandFirst(),
                CommandType.Second => new CybershokeCommandSecond(value),
                CommandType.Third => new CybershokeCommandThird(customId, value),
                _ => throw new NotImplementedException()
            };
        }
    }
}
