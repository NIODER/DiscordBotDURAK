using System;

namespace DiscordBotDurak.Exceptions
{
    public abstract class DiscordBotException : Exception
    {
        public string Service { get; private set; }

        protected DiscordBotException(string service)
        {
            Service = service;
        }

        protected DiscordBotException(string message, string service) : base(message)
        {
            Service = service;
        }

        public override string ToString()
        {
            return $"Service: {Service}.\n{base.ToString()}";
        }
    }
}
