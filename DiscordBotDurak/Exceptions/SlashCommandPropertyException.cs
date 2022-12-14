using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DiscordBotDurak.Exceptions
{
    public class SlashCommandPropertyException : DiscordBotSlashCommandException
    {
        public string PropertyName { get; private set; }

        public SlashCommandPropertyException(string propertyName, string slashCommandName) : base(slashCommandName)
        {
            PropertyName = propertyName;
        }

        public SlashCommandPropertyException(string propertyName, string slashCommandName, string message) : base(slashCommandName, message)
        {
            PropertyName = propertyName;
        }
    }
}
