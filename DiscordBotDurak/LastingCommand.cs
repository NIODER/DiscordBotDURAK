using DiscordBotDurak.Enum.Commands;
using System;
using System.Threading;

namespace DiscordBotDurak
{
    public abstract class LastingCommand
    {
        public ulong AuthorId { get; private set; }
        public ulong GuildId { get; private set; }
        public LastingCommandsObserver Observer { get; }
        private CancellationTokenSource CancellationTokenSource { get; }
        protected CancellationToken CancellationToken { get; }
        public CommandType CommandType { get; set; }

        protected LastingCommand(ulong authorId, ulong guildId, CommandType commandType)
        {
            Observer = LastingCommandsObserver.Instance();
            CancellationTokenSource = new CancellationTokenSource();
            CancellationToken = CancellationTokenSource.Token;
            AuthorId = authorId;
            GuildId = guildId;
            CommandType = commandType;
            Observer.Add(this);
        }

        /// <summary>
        /// Execute if command called with exception
        /// </summary>
        protected LastingCommand()
        {
        }

        public void Interrupt()
        {
            CancellationTokenSource.Cancel();
            Observer.Remove(this);
        }

        public void Finished()
        {
            Observer.Remove(this);
        }
    }
}
