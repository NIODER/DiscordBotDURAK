using DatabaseModel;
using DatabaseModel.Context;
using Discord;
using DiscordBotDurak.Enum.ModerationModes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBotDurak.Data
{
    /// <summary>
    /// На самом деле, я думаю, что этот класс - безобразие.
    /// </summary>
    public class Database : IDisposable
    {
        private static readonly object syncObj = new object();
        public Exception Exception { get; private set; }
        public DatabaseContext Context { get; private set; }
        public IDbContextTransaction Transaction { get; private set; }

        public Database()
        {
            Context = new DatabaseContext(JObject.Parse(File.ReadAllText(Program.resourcesPath + "/config.json"))["dbconstring"]?.ToString()
                ?? throw new ArgumentNullException("connectionString", "no such attribute"));
            Transaction = null;
            Exception = null;
        }

        public Database(string connectionString)
        {
            Context = new DatabaseContext(connectionString);
            Transaction = null;
            Exception = null;
        }

        public IDbContextTransaction BeginTransaction()
        {
            return Transaction ??= Context.Database.BeginTransaction();
        }

        public void DropTransaction()
        {
            Logger.Log(LogSeverity.Info, GetType().Name, $"{Transaction} dropped");
            Transaction = null;
            Exception = null;
        }

        /// <returns>null if this user already in database</returns>
        public GuildUser AddUser(GuildUser user)
        {
            
            var user1 = Context.GuildUsers.Add(user);
            Context.SaveChanges();
            Logger.Log(LogSeverity.Info, GetType().Name, $"User added {user1.Entity}");
            return user1.Entity;
        }

        public GuildUser GetUser(ulong guildId, ulong userId)
        {
            return Context.Guilds.AsQueryable()
                .Include(g => g.GuildUsers)
                .FirstOrDefault(g => g.GuildId == guildId)
                ?.GuildUsers?.AsQueryable()
                ?.FirstOrDefault(u => u.UserId == userId);
        }

        public GuildUser UpdateUser(GuildUser user)
        {
            var user1 = Context.GuildUsers.Update(user);
            Logger.Log(LogSeverity.Info, GetType().Name, $"User updated old:\n{user}\nnew:\n{user1.Entity}");
            return user1.Entity;
        }

        public GuildUser DeleteUser(GuildUser user)
        {
            Logger.Log(LogSeverity.Info, GetType().Name, $"User deleted {user}");
            return Context.GuildUsers.Remove(user).Entity;
        }

        public GuildUser DeleteUser(ulong guildId, ulong userId)
        {
            var user = GetUser(guildId, userId);
            return DeleteUser(user);
        }

        public Channel AddChannel(Channel channel)
        {
            
            var channel1 = Context.Channels.Add(channel);
            Context.SaveChanges();
            Logger.Log(LogSeverity.Info, GetType().Name, $"Channel added {channel1.Entity}");
            return channel1.Entity;
        }

        public Channel GetChannel(ulong channelId)
        {
            return Context.Channels.AsQueryable()
                .Include(c => c.GuildNavigation)
                .Include(c => c.SymbolsLists)
                .FirstOrDefault(c => c.ChannelId == channelId);
        }

        public Channel UpdateChannel(Channel channel)
        {
            var channel1 = Context.Channels.Update(channel);
            Logger.Log(LogSeverity.Info, GetType().Name, $"Channel updated old:\n{channel}\nnew:\n{channel1.Entity}");
            return channel1.Entity;
        }

        public Channel DeleteChannel(Channel channel)
        {
            Logger.Log(LogSeverity.Info, GetType().Name, $"Channel deleted {channel}");
            return Context.Channels.Remove(channel).Entity;
        }

        public Guild AddGuild(Guild guild)
        {
            
            var guild1 = Context.Guilds.Add(guild);
            Context.SaveChanges();
            Logger.Log(LogSeverity.Info, GetType().Name, $"Guild added {guild1.Entity}");
            return guild1.Entity;
        }   

        public Guild GetGuild(ulong guildId)
        {
            return Context.Guilds.FirstOrDefault(g => g.GuildId == guildId);
        }

        public Guild UpdateGuild(Guild guild)
        {
            var guild1 = Context.Guilds.Update(guild);
            Logger.Log(LogSeverity.Info, GetType().Name, $"Guild updated old:\n{guild}\nnew:\n{guild1.Entity}");
            return guild1.Entity;
        }

        public Guild DeleteGuild(Guild guild)
        {
            Logger.Log(LogSeverity.Info, GetType().Name, $"Guild deleted {guild}");
            return Context.Guilds.Remove(guild).Entity;
        }

        public void AddSymbolsListToChannel(
            ulong symbolsListId,
            ulong channelId,
            ModerationMode moderationMode = ModerationMode.NonModerated)
        {
            var list = Context.SymbolsLists.FirstOrDefault(sl => sl.ListId == symbolsListId);
            Logger.Log(LogSeverity.Info,
                GetType().Name,
                $"SymbolsList added to channel with moderation mode: {moderationMode}, " +
                $"SymbolList {list}, channel id: {channelId}");
            Context.SymbolsListsToChannels.Add(new SymbolsListsToChannels
            {
                ChannelId = channelId,
                ListId = symbolsListId,
                Moderation = (short)moderationMode
            });
        }

        public void AddSymbolsListToChannel(
            ulong symbolsListId,
            ulong channelId,
            ulong resendChannelId)
        {
            var list = Context.SymbolsLists.FirstOrDefault(sl => sl.ListId == symbolsListId);
            Logger.Log(
                LogSeverity.Info, 
                GetType().Name, 
                $"SymbolsList added to channel with moderation mode:\n{ModerationMode.OnlyResend},\n" +
                $"SymbolList {list},\nchannel id: {channelId},\nresend channel id: {resendChannelId}");
            Context.SymbolsListsToChannels.Add(new SymbolsListsToChannels()
            {
                ChannelId = channelId,
                ListId = symbolsListId,
                Moderation = (short)ModerationMode.OnlyResend,
                ResendChannelId = resendChannelId
            });
        }

        /// <returns>false if symbolsList with id "symbolsListId" not found</returns>
        public bool AddSymbolsListToChannel(
            ulong symbolsListId,
            ulong channelId,
            ModerationMode moderationMode = ModerationMode.NonModerated,
            ulong? resendChannelId = null)
        {
            var symbolsList = Context.SymbolsLists.FirstOrDefault(sl => sl.ListId == symbolsListId);
            if (symbolsList is null) return false;
            var channel = Context.Channels.FirstOrDefault(c => c.ChannelId == channelId);
            AddSymbolsListToGuild(channel.GuildId, symbolsList);
            if (!Utilities.CodeContains(channel.Moderation, moderationMode))
                channel.Moderation |= (short)moderationMode;
            UpdateChannel(channel);
            if (resendChannelId is not null)
                AddSymbolsListToChannel(symbolsList.ListId, channelId, (ulong)resendChannelId);
            else AddSymbolsListToChannel(symbolsList.ListId, channelId, moderationMode);
            return true;
        }

        /// <returns>false if symbolsList with id "symbolsListId" not found</returns>
        public bool DeleteSymbolsListFromChannel(ulong symbolsListId, ulong channelId)
        {
            var symbolsListToChannel = Context.SymbolsListsToChannels
                .FirstOrDefault(sltc => sltc.ListId == symbolsListId && sltc.ChannelId == channelId);
            if (symbolsListToChannel is not null)
            {
                Context.SymbolsListsToChannels.Remove(symbolsListToChannel);
                Logger.Log(LogSeverity.Info, GetType().Name,
                    $"SymbolsList deleted from channel list id: {symbolsListId}, channel id: {channelId}.");
                return true;
            }
            else
                Logger.Log(LogSeverity.Info, GetType().Name,
                    $"SymbolsList is not found in this channel. list id: {symbolsListId}, channel id: {channelId}.");

            return false;
        }

        /// <returns>true if success. false is guild not found or
        /// if there are no symbolsList with id symbolsListId connected
        /// with guild</returns>
        public bool DeleteSymbolsListFromGuild(ulong symbolsListId, ulong guildId)
        {
            Logger.Log(LogSeverity.Info, GetType().Name,
                $"SymbolsList deleted from guild list id: {symbolsListId}, guild id: {guildId}.");
            var symbolsList = Context.SymbolsLists.AsQueryable()
                .FirstOrDefault(sl => sl.ListId == symbolsListId);
            if (symbolsList is null) return true;
            return Context.Guilds.Include(g => g.SymbolsLists)
                .FirstOrDefault(g => g.GuildId == guildId)
                ?.SymbolsLists?.Remove(symbolsList) ?? false;
        }

        public SymbolsList CreateSymbolsList(string title = null)
        {
            var symbolsList = new SymbolsList()
            {
                Title = title ?? "Untitled"
            };
            
            var symbolsList1 = Context.SymbolsLists.Add(symbolsList);
            Context.SaveChanges();
            Logger.Log(LogSeverity.Info, GetType().Name,
                $"SymbolsList created {symbolsList1.Entity}.");
            return symbolsList1.Entity;
        }

        /// <returns>false if symbolsList is null or if there are no guild with id guildId</returns>
        public bool AddSymbolsListToGuild(ulong guildId, SymbolsList symbolsList)
        {
            if (symbolsList is null) return false;
            var guild = Context.Guilds.Include(g => g.SymbolsLists)
                .FirstOrDefault(g => g.GuildId == guildId);
            if (guild is null) return false;
            guild.SymbolsLists.Add(symbolsList);
            Logger.Log(LogSeverity.Info, GetType().Name,
                $"SymbolsList added to guild {symbolsList},\nguild id: {guildId}.");
            return true;
        }

        public SymbolsList CreateSymbolsListAndAddToGuild(ulong guildId, string title)
        {
            var symbolsList = CreateSymbolsList(title);
            return AddSymbolsListToGuild(guildId, symbolsList) ? symbolsList : null;
        }
        
        public SymbolsList GetSymbolsList(ulong symbolsListId)
        {
            return Context.SymbolsLists.Include(sl => sl.Guilds).AsQueryable()
                .FirstOrDefault(sl => sl.ListId == symbolsListId);
        }
        
        public SymbolsList GetSymbolsList(ulong symbolsListId, ulong guildId)
        {
            return Context.SymbolsLists.Include(sl => sl.Guilds).AsQueryable()
                .Where(sl => sl.ListId == symbolsListId)
                .FirstOrDefault(sl => sl.Guilds.Contains(GetGuild(guildId)));
        }

        public List<SymbolsList> GetSymbolsLists(ulong channelId, ModerationMode moderationMode)
        {
            return Context.SymbolsListsToChannels.AsQueryable()
                .Where(sltc => sltc.ChannelId == channelId)
                .Where(sltc => sltc.Moderation == (short)moderationMode)
                .Select(sltc => sltc.SymbolsListNavigation).ToList();
        }

        public List<SymbolsList> GetSymbolsLists(ulong channelId)
        {
            return Context.SymbolsListsToChannels.AsQueryable()
                .Where(sltc => sltc.ChannelId == channelId)
                .Select(sltc => sltc.SymbolsListNavigation).ToList();
        }

        public List<SymbolObject> GetChannelBanSymbols(ulong channelId)
        {
            return Context.SymbolsListsToChannels.AsQueryable()
                .Where(sltc => sltc.ChannelId == channelId)
                .Join(Context.SymbolsLists.AsQueryable(),
                      sltc => sltc.ListId,
                      sl => sl.ListId,
                      (sltc, sl) => sl)
                .Join(Context.SymbolsListsToSymbols.AsQueryable(),
                      sl => sl.ListId,
                      slts => slts.ListId,
                      (sl, slts) => new { slts.SymbolId, slts.IsExcluded, sl.ListId })
                .Join(Context.Symbols.AsQueryable(),
                      slts => slts.SymbolId,
                      s => s.SymbolId,
                      (slts, s) => new SymbolObject(s.SymbolId, s.Content, slts.IsExcluded, slts.ListId))
                .ToList();
        }


        public ulong? GetResendChannelId(ulong channelId, ulong symbolsListId)
        {
            return Context.SymbolsListsToChannels.AsQueryable()
                .FirstOrDefault(sltc => sltc.ChannelId == channelId && sltc.ListId == symbolsListId)
                ?.ResendChannelId;
        }
        /// <summary>
        /// Add symbol to symbols list or update <paramref name="isExcluded"/> in existing one.
        /// </summary>
        /// <param name="symbolsList"></param>
        /// <param name="symbol"></param>
        /// <param name="isExcluded"></param>
        public void AddSymbolToBanwordList(SymbolsList symbolsList, Symbol symbol, bool isExcluded)
        {
            var slts1 = Context.SymbolsListsToSymbols.FirstOrDefault(s => s.SymbolId == symbol.SymbolId && s.ListId == symbolsList.ListId);
            if (slts1 is null)
            {
                Context.SymbolsLists.AsQueryable()
                    .Include(sl => sl.Symbols)
                    .FirstOrDefault(sl => sl.ListId == symbolsList.ListId)
                    ?.Symbols.Add(Context.Symbols.AsQueryable().FirstOrDefault(s => s.SymbolId == symbol.SymbolId));
                Context.SaveChanges();
                Logger.Log(LogSeverity.Info, GetType().Name, $"Symbol added to BanwordList.");
            }
            else
            {
                slts1.IsExcluded = isExcluded;
                Context.SaveChanges();
                Logger.Log(LogSeverity.Info, GetType().Name, $"Symbol excluded parameter was updated.");
            }
        }

        public void AddSymbolToBanwordList(ulong symbolsListId, ulong symbolId, bool isExcluded)
        {
            var slts1 = Context.SymbolsListsToSymbols.FirstOrDefault(s => s.SymbolId == symbolId && s.ListId == symbolsListId);
            if (slts1 is null)
            {
                Context.SymbolsLists.AsQueryable()
                    .Include(sl => sl.Symbols)
                    .FirstOrDefault(sl => sl.ListId == symbolsListId)
                    ?.Symbols.Add(Context.Symbols.AsQueryable().FirstOrDefault(s => s.SymbolId == symbolId));
                Context.SaveChanges();
                Logger.Log(LogSeverity.Info, GetType().Name, $"Symbol added to BanwordList.");
            }
            else
            {
                slts1.IsExcluded = isExcluded;
                Context.SaveChanges();
                Logger.Log(LogSeverity.Info, GetType().Name, $"Symbol excluded parameter was updated.");
            }
        }

        public void RemoveSymbolFromSymbolsList(ulong symbolsListId, ulong symbolId)
        {
            var symbolsListToSymbols = Context.SymbolsListsToSymbols.AsQueryable()
                .FirstOrDefault(slts => slts.SymbolId == symbolId && slts.ListId == symbolsListId);
            int rowsCount = Context.SymbolsListsToSymbols.AsQueryable()
                .Where(slts => slts.SymbolId == symbolId && slts.ListId == symbolsListId)
                .ExecuteDelete();
            Logger.Log(LogSeverity.Info, GetType().Name, $"Symbol removed from BanwordList: {symbolsListToSymbols}. Rows affected {rowsCount}.");
        }

        public void DeleteSymbolsList(SymbolsList symbolsList)
        {
            Logger.Log(LogSeverity.Info, GetType().Name, $"SymbolsList removed: {symbolsList}");
            Context.SymbolsLists.Remove(symbolsList);
        }

        public void DeleteSymbolsList(ulong symbolsListId)
        {
            var symbolsList = GetSymbolsList(symbolsListId);
            DeleteSymbolsList(symbolsList);
        }

        public Symbol AddSymbol(string content)
        {
            var symbol = new Symbol()
            {
                Content = content
            };
            
            var symbol1 = Context.Symbols.Add(symbol);
            Context.SaveChanges();
            Logger.Log(LogSeverity.Info, GetType().Name, $"Symbol added: {symbol1}");
            return symbol1.Entity;
        }

        public List<Symbol> GetSymbols(ulong symbolsListId)
        {
            return Context.SymbolsLists.Include(sl => sl.Symbols)
                .FirstOrDefault(sl => sl.ListId == symbolsListId)?.Symbols;
        }

        public Symbol FindSymbol(string symbol)
        {
            return Context.Symbols.FirstOrDefault(s => s.Content == symbol);
        }

        public void DeleteSymbol(ulong symbolId)
        {
            var symbol = Context.Symbols.FirstOrDefault(s => s.SymbolId == symbolId);
            if (symbol is not null)
            {
                symbol = Context.Symbols.Remove(symbol).Entity;
                Logger.Log(LogSeverity.Info, GetType().Name, $"Symbol deleted: {symbol}");
            }
            Logger.Log(LogSeverity.Info, GetType().Name, $"Symbol not found in db on deletion: {symbolId}");
        }

        public List<SymbolsListsToChannels> GetSymbolsListsToChannel(ulong channelId)
        {
            return Context.Channels.Include(c => c.SymbolsListsToChannels).AsQueryable()
                .FirstOrDefault(c => c.ChannelId == channelId)
                ?.SymbolsListsToChannels?.ToList();
        }

        public SymbolsListsToChannels GetSymbolsListToChannel(ulong channelId, ulong listId)
        {
            return Context.SymbolsListsToChannels.AsQueryable()
                .Include(sltc => sltc.SymbolsListNavigation)
                .Include(sltc => sltc.ChannelNavigation)
                .FirstOrDefault(sltc => sltc.ListId == listId && sltc.ChannelId == channelId);
        }

        public SymbolsListsToChannels UpdateSymbolsListToChannel(SymbolsListsToChannels symbolsListsToChannels)
        {
            var symbolsListToChannels1 = Context.SymbolsListsToChannels.Update(symbolsListsToChannels);
            Logger.Log(LogSeverity.Info, GetType().Name, $"SymbolsListToChannel updated old:\n{symbolsListsToChannels}\nnew:\n{symbolsListToChannels1.Entity}.");
            return symbolsListToChannels1.Entity;
        }

        public ModerationMode GetModerationMode(ulong channelId, ulong symbolsListId)
        {
            return (ModerationMode)(Context.SymbolsListsToChannels.AsQueryable()
                .FirstOrDefault(sltc => sltc.ChannelId == channelId && sltc.ListId == symbolsListId)
                ?.Moderation);
        }

        public List<SymbolsListsToSymbols> GetSymbolsListsToSymbols(ulong listId)
        {
            return Context.SymbolsListsToSymbols.AsQueryable()
                .Include(slts => slts.SymbolNavigation)
                .Where(slts => slts.ListId == listId)?.ToList();
        }

        public List<SymbolsList> GetGuildSymbolsLists(ulong guildId)
        {
            return Context.Guilds.AsQueryable()
                .Include(g => g.SymbolsLists)
                .FirstOrDefault(g => g.GuildId == guildId)
                ?.SymbolsLists.ToList();
        }

        public List<Channel> GetChannelsContainsList(ulong guildId, ulong listId)
        {
            return Context.Channels.AsQueryable()
                .Include(c => c.SymbolsLists)
                .Where(c => c.GuildId == guildId).AsQueryable()
                .Where(c => c.SymbolsLists.Contains(Context.SymbolsLists.First(sl => sl.ListId == listId)))
                .ToList();
        }

        public short GetUpdatedChannelModeration(ulong channelId)
        {
            short moderation = 0;
            Context.SymbolsListsToChannels.AsQueryable()
                .Where(sltc => sltc.ChannelId == channelId)
                .Select(sltc => sltc.Moderation).ToList()
                .ForEach(m => moderation |= m);
            Context.Channels.AsQueryable().First(c => c.ChannelId == channelId).Moderation = moderation;
            Context.SaveChanges();
            return moderation;
        }

        public GuildUser GetUserByQuestion(ulong qMessageId, ulong userId)
        {
            return Context.GuildUsers
                .Include(u => u.GuildNavigation)
                .FirstOrDefault(u => u.UserId == userId && u.QMessageId == qMessageId);
        }

        /// <summary>
        /// Commits changes in database async.
        /// </summary>
        /// <returns>true if successfully</returns>
        public async Task<bool> CommitAsync()
        {
            bool successfully = true;
            if (Transaction is not null)
            {
                try
                {
                    await Context.SaveChangesAsync();
                    await Transaction.CommitAsync();
                }
                catch (Exception e)
                {
                    Exception = e;
                    Logger.Log(LogSeverity.Info, GetType().Name, $"Transaction commit exception.", Exception);
                    await Transaction.RollbackAsync();
                    successfully = false;
                }
                Transaction.Dispose();
                Transaction = null;
            }
            else
            {
                Exception = new ArgumentNullException("Transaction was null");
                Logger.Log(LogSeverity.Info, GetType().Name, $"Transaction commit exception.", Exception);
                successfully = false;
            }
            Logger.Log(LogSeverity.Info, GetType().Name, $"Transaction commited");
            return successfully;
        }


        /// <summary>
        /// Thread-safely commits changes in database. 
        /// </summary>
        /// <returns>true if successfully</returns>
        public bool CommitSychronized()
        {
            bool successfully = true;
            if (Transaction is not null)
            {
                lock (syncObj)
                {
                    try
                    {
                        Context.SaveChanges();
                        Transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        Exception = e;
                        Logger.Log(LogSeverity.Info, GetType().Name, 
                            $"Transaction sychronized commit exception.", Exception);
                        Transaction.Rollback();
                        successfully = false;
                    }
                }
            }
            else
            {
                Exception = new ArgumentNullException("Transaction was null");
                Logger.Log(LogSeverity.Info, GetType().Name,
                    $"Transaction sychronized commit exception.", Exception);
                successfully = false;
            }
            Transaction = null;
            Logger.Log(LogSeverity.Info, GetType().Name, $"Transaction syncronized commited");
            return successfully;
        }

        public void Dispose()
        {
            Transaction?.Dispose();
            Context.Dispose();
        }
    }
}
