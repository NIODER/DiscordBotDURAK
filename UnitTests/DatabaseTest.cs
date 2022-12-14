using DatabaseModel;
using DiscordBotDurak.Data;
using DiscordBotDurak.Enum.BotRoles;
using DiscordBotDurak.Enum.ModerationModes;
using Microsoft.EntityFrameworkCore;
using Npgsql.Internal;

namespace DiscordBotDurak.Tests
{
    public class DatabaseTest
    {
        public const string TESTING_CONNECTION_STRING =
            "Host=localhost;Port=5432;Database=TestingDatabase;Username=postgres;Password=nioder125;Include Error Detail=true";

        [Fact]
        public void AddUserTest()
        {
            using var db = new Database(TESTING_CONNECTION_STRING);
            var guild = db.Context.Guilds.First(g => g.GuildId == 2);
            var user = new GuildUser()
            {
                UserId = 123,
                GuildNavigation = guild
            };
            db.BeginTransaction();
            db.AddUser(user);
            if (!db.CommitAsync().Result) 
                throw db.Exception;
            db.BeginTransaction();
            Assert.True(db.Context.GuildUsers.Contains(user));
        }

        [Fact]
        public void GetUserTest()
        {
            using var db = new Database(TESTING_CONNECTION_STRING);
            Assert.NotNull(db.GetUser(1, 1));
        }

        [Fact]
        public void UpdateUserTest()
        {
            const string newInvitedString = "By John Doe";
            using var db = new Database(TESTING_CONNECTION_STRING);
            var guild = db.Context.Guilds.AsQueryable().First(g => g.GuildId == 1);
            var user = new GuildUser()
            {
                UserId = 123,
                GuildNavigation = guild
            };
            db.Context.GuildUsers.Add(user); 
            db.Context.SaveChanges();
            user.Invited = newInvitedString;
            db.BeginTransaction();
            db.UpdateUser(user);
            if (!db.CommitAsync().Result)
                throw db.Exception;
            user = db.Context.Guilds.AsQueryable().First(g => g.GuildId == 1)
                .GuildUsers.First(u => u.UserId == 123);
            Assert.Equal(newInvitedString, user.Invited);
        }

        [Fact]
        public void DeleteUserTest()
        {
            using var db = new Database(TESTING_CONNECTION_STRING);
            var guild = db.Context.Guilds.AsQueryable().First(g => g.GuildId == 1);
            var user = new GuildUser()
            {
                UserId = 1234,
                GuildNavigation = guild
            };
            db.Context.GuildUsers.Add(user);
            db.Context.SaveChanges();
            db.BeginTransaction();
            db.DeleteUser(user);
            if (!db.CommitAsync().Result)
                throw db.Exception;
            Assert.Null(db.Context.GuildUsers.AsQueryable()
                .Where(u => u.GuildId == user.GuildId)
                .FirstOrDefault(u => u.UserId == user.UserId));
        }

        [Fact]
        public void AddChannelTest()
        {
            using var db = new Database(TESTING_CONNECTION_STRING);
            var guild = db.Context.Guilds.AsQueryable().First(g => g.GuildId == 1);
            var channel = new Channel()
            {
                ChannelId = 12319,
                GuildNavigation = guild
            };
            db.BeginTransaction();
            db.AddChannel(channel);
            if (!db.CommitAsync().Result)
                throw db.Exception;
            channel = db.Context.Channels.AsQueryable()
                .Where(c => c.GuildId == guild.GuildId)
                .FirstOrDefault(c => c.ChannelId == 12319);
            Assert.NotNull(channel);
        }

        [Fact]
        public void GetChannelTest()
        {
            using var db = new Database(TESTING_CONNECTION_STRING);
            var guild = db.Context.Guilds.AsQueryable().First(g => g.GuildId == 1);
            var channel = new Channel()
            {
                ChannelId = 1234,
                GuildNavigation = guild
            };
            db.Context.Channels.Add(channel);
            db.Context.SaveChanges();
            Assert.NotNull(db.GetChannel(channel.ChannelId));
        }

        [Fact]
        public void UpdateChannelTest()
        {
            using var db = new Database(TESTING_CONNECTION_STRING);
            var guild = db.Context.Guilds.AsQueryable().First(g => g.GuildId == 1);
            var channel = new Channel()
            {
                ChannelId = 1234555,
                GuildNavigation = guild
            };
            db.Context.Channels.Add(channel);
            db.Context.SaveChanges();
            channel.Moderation = Channel.DEFAULT_MODERATION + 1;
            db.BeginTransaction();
            db.UpdateChannel(channel);
            if (!db.CommitAsync().Result)
                throw db.Exception;
            channel = db.Context.Channels.AsQueryable().FirstOrDefault(c => c.ChannelId == channel.ChannelId);
            Assert.Equal(Channel.DEFAULT_MODERATION + 1, channel?.Moderation ?? -1);
        }

        [Fact]
        public void DeleteChannelTest()
        {
            using var db = new Database(TESTING_CONNECTION_STRING);
            var guild = db.Context.Guilds.AsQueryable().First(g => g.GuildId == 1);
            var channel = new Channel()
            {
                ChannelId = 123456,
                GuildNavigation = guild
            };
            db.Context.Channels.Add(channel);
            db.Context.SaveChanges();
            db.BeginTransaction();
            db.DeleteChannel(channel);
            if (!db.CommitAsync().Result)
                throw db.Exception;
            channel = db.Context.Channels.AsQueryable().FirstOrDefault(c => c.ChannelId == channel.ChannelId);
            Assert.Null(channel);
        }

        [Fact]
        public void AddGuildTest()
        {
            using var db = new Database(TESTING_CONNECTION_STRING);
            var guild = new Guild()
            {
                GuildId = 12300
            };
            db.BeginTransaction();
            db.AddGuild(guild);
            if (!db.CommitAsync().Result)
                throw db.Exception;
            Assert.True(db.Context.Guilds.AsQueryable().Contains(guild));
        }

        [Fact]
        public void GetGuildTest()
        {
            using var db = new Database(TESTING_CONNECTION_STRING);
            var guild = new Guild()
            {
                GuildId = 1234
            };
            db.Context.Guilds.Add(guild);
            db.Context.SaveChanges();
            Assert.NotNull(db.GetGuild(guild.GuildId));
        }

        [Fact]
        public void UpdateGuildTest()
        {
            using var db = new Database(TESTING_CONNECTION_STRING);
            var guild = new Guild()
            {
                GuildId = 12345
            };
            db.Context.Guilds.Add(guild);
            db.Context.SaveChanges();
            guild.BaseRole = Guild.DEFAULT_BASE_ROLE + 1;
            db.BeginTransaction();
            db.UpdateGuild(guild);
            if (!db.CommitAsync().Result)
                throw db.Exception;
            guild = db.Context.Guilds.AsQueryable().First(g => g.GuildId == guild.GuildId);
            Assert.Equal(Guild.DEFAULT_BASE_ROLE + 1, guild.BaseRole);
        }

        [Fact]
        public void DeleteGuildTest()
        {
            using var db = new Database(TESTING_CONNECTION_STRING);
            var guild = new Guild()
            {
                GuildId = 123456
            };
            db.Context.Guilds.Add(guild);
            db.Context.SaveChanges();
            db.BeginTransaction();
            db.DeleteGuild(guild);
            if (!db.CommitAsync().Result)
                throw db.Exception;
            guild = db.Context.Guilds.AsQueryable().FirstOrDefault(g => g.GuildId == guild.GuildId);
            Assert.Null(guild);
        }

        [Fact]
        public void AddSymbolsListToChannelTest()
        {
            using var db = new Database(TESTING_CONNECTION_STRING);
            var symbolsList = new SymbolsList()
            {
                Title = "Moderation"
            };
            var symbolsList1 = new SymbolsList()
            {
                Title = "Resend"
            };
            symbolsList = db.Context.SymbolsLists.Add(symbolsList).Entity;
            symbolsList1 = db.Context.SymbolsLists.Add(symbolsList1).Entity;
            db.Context.SaveChanges();
            var channel = db.Context.Channels.AsQueryable().First(c => c.ChannelId == 1);
            db.BeginTransaction();
            db.AddSymbolsListToChannel(symbolsList.ListId, channel.ChannelId, ModerationMode.OnlyDelete);
            db.AddSymbolsListToChannel(symbolsList1.ListId, channel.ChannelId, 12341234);
            if (!db.CommitAsync().Result)
                throw db.Exception;
            channel = db.Context.Channels
                .Include(c => c.SymbolsLists).AsQueryable()
                .First(c => c.ChannelId == 1);
            Assert.Contains(symbolsList, channel.SymbolsLists);
            Assert.Contains(symbolsList1, channel.SymbolsLists);
        }

        [Fact]
        public void AddSymbolsListToChannelResendSetTest()
        {
            using var db = new Database(TESTING_CONNECTION_STRING);
            var symbolsList = new SymbolsList()
            {
                ListId = 123123000
            };
            db.Context.SymbolsLists.Add(symbolsList);
            db.Context.SaveChanges();
        }

        [Fact]
        public void DeleteSymbolsListFromChannelTest()
        {
            using var db = new Database(TESTING_CONNECTION_STRING);
            var symbolsList = new SymbolsList()
            {
                ListId = 12312312333
            };
            db.Context.SymbolsLists.Add(symbolsList);
            db.Context.SaveChanges();
            var channel = db.Context.Channels
                .Include(c => c.SymbolsLists).AsQueryable()
                .First(c => c.ChannelId == 1);
            channel.SymbolsLists.Add(symbolsList);
            db.Context.SaveChanges();
            db.BeginTransaction();
            bool deleted = db.DeleteSymbolsListFromChannel(symbolsList.ListId, channel.ChannelId);
            if (!db.CommitAsync().Result)
                throw db.Exception;
            channel = db.Context.Channels
                .Include(c => c.SymbolsLists).AsQueryable()
                .First(c => c.ChannelId == 1);
            Assert.True(deleted);
            Assert.DoesNotContain(symbolsList, channel.SymbolsLists);
        }

        [Fact]
        public void DeleteSymbolsListFromGuildTest()
        {
            using var db = new Database(TESTING_CONNECTION_STRING);
            var guild = new Guild()
            {
                GuildId = 1234567
            };
            var symbolsList = new SymbolsList()
            {
                ListId = 123123123
            };
            db.Context.Guilds.Add(guild);
            db.Context.SymbolsLists.Add(symbolsList);
            db.Context.SaveChanges();
            db.Context.Guilds.Include(g => g.SymbolsLists).AsQueryable()
                .First(g => g.GuildId == guild.GuildId)
                .SymbolsLists.Add(symbolsList);
            db.Context.SaveChanges();
            bool deleted = db.DeleteSymbolsListFromGuild(symbolsList.ListId, guild.GuildId);
            guild = db.Context.Guilds
                .Include(g => g.SymbolsLists).AsQueryable()
                .First(g => g.GuildId == guild.GuildId);
            Assert.True(deleted);
            Assert.DoesNotContain(symbolsList, guild.SymbolsLists);
        }

        [Fact]
        public void CreateSymbolsListTest()
        {
            using var db = new Database(TESTING_CONNECTION_STRING);
            db.BeginTransaction();
            var symbolsList = db.CreateSymbolsList("TestingList");
            if (!db.CommitAsync().Result)
                throw db.Exception;
            var dbSymbolsList = db.Context.SymbolsLists.AsQueryable()
                .First(sl => sl.ListId == symbolsList.ListId);
            Assert.Equivalent(symbolsList, dbSymbolsList);
            Assert.Equal(symbolsList, dbSymbolsList);
        }

        [Fact]
        public void AddSymbolsListToGuildTest()
        {
            const ulong listId = 6;
            using var db = new Database(TESTING_CONNECTION_STRING);
            var symbolsList = db.Context.SymbolsLists.First(sl => sl.ListId == listId);
            db.BeginTransaction();
            bool added = db.AddSymbolsListToGuild(1, symbolsList);
            if (!db.CommitAsync().Result)
                throw db.Exception;
            var guild = db.Context.Guilds
                .Include(g => g.SymbolsLists).AsQueryable()
                .First(g => g.GuildId == 1);
            Assert.True(added);
            Assert.Contains(symbolsList, guild.SymbolsLists);
        }

        [Fact]
        public void CreateSymbolsListAndAddToGuildTest()
        {
            const ulong guildId = 1;
            using var db = new Database(TESTING_CONNECTION_STRING);
            db.BeginTransaction();
            var symbolsList = db.CreateSymbolsListAndAddToGuild(guildId, "CreateSymbolsListAndAddToGuildTest");
            if (!db.CommitAsync().Result)
                throw db.Exception;
            Assert.NotNull(symbolsList);
            Assert.Contains(symbolsList, db.Context.SymbolsLists.AsQueryable());
            Assert.Contains(symbolsList, db.Context.Guilds.Include(g => g.SymbolsLists)
                .First(g => g.GuildId == guildId).SymbolsLists.AsQueryable());
        }

        [Fact]
        public void GetSymbolsListTest()
        {
            using var db = new Database(TESTING_CONNECTION_STRING);
            var symbolsList = new SymbolsList()
            {
                ListId = 11,
                Title = "GetSymbolsListTest"
            };
            db.Context.SymbolsLists.Add(symbolsList);
            db.Context.SaveChanges();
            var dbSymbolsList = db.GetSymbolsList(symbolsList.ListId);
            db.BeginTransaction();
            db.AddSymbolsListToChannel(symbolsList.ListId, 1, 11111111111111);
            if (!db.CommitAsync().Result)
                throw db.Exception;
            var lists = db.GetSymbolsLists(1, ModerationMode.OnlyResend);
            Assert.NotNull(dbSymbolsList);
            Assert.Equivalent(symbolsList, dbSymbolsList);
            Assert.Equivalent(lists, db.Context.Channels.Include(c => c.SymbolsLists)
                .First(c => c.ChannelId == 1)
                .SymbolsLists);
        }

        [Fact]
        public void UpdateSymbolsListToChannel()
        {
            using var db = new Database(TESTING_CONNECTION_STRING);
            var sltc = db.Context.SymbolsListsToChannels.First(sltc => sltc.ListId == 3 && sltc.ChannelId == 3);
            Assert.NotEqual((short)ModerationMode.OnlyDelete, sltc.Moderation);
            sltc.Moderation = (short)ModerationMode.OnlyDelete;
            db.BeginTransaction();
            var sltc1 = db.UpdateSymbolsListToChannel(sltc);
            if (!db.CommitAsync().Result)
            {
                throw db.Exception;
            }
            Assert.Equal((short)ModerationMode.OnlyDelete, sltc1.Moderation);
        }

        [Fact]
        public void GetGuildSymbolsListTest()
        {
            const ulong guildId = 1;
            using var db = new Database(TESTING_CONNECTION_STRING);
            var symbolsList = new SymbolsList()
            {
                ListId = 12,
                Title = "GetGuildSymbolsListTest"
            };
            db.Context.SymbolsLists.Add(symbolsList);
            db.Context.SaveChanges();
            db.Context.Guilds
                .Include(g => g.SymbolsLists).AsQueryable()
                .First(g => g.GuildId == guildId)
                .SymbolsLists.Add(symbolsList);
            db.Context.SaveChanges();
            Assert.NotNull(db.GetSymbolsList(symbolsList.ListId, guildId));
        }

        [Fact]
        public void GetChannelBanwordlistsTest()
        {
            const ulong channelId = 4;
            using var db = new Database(TESTING_CONNECTION_STRING);
            var symbolsLists = db.Context.Channels.AsQueryable()
                .Include(c => c.SymbolsLists)
                .First(c => c.ChannelId == channelId)
                .SymbolsLists;
            var dbSymbolsList = db.GetSymbolsLists(channelId);
            Assert.NotNull(dbSymbolsList);
            Assert.NotNull(symbolsLists);
            Assert.NotEmpty(symbolsLists);
            Assert.Equivalent(symbolsLists, dbSymbolsList, true);
        }

        [Fact]
        public void GetChannelSymbolsTest()
        {
            const ulong channelId = 4;
            using var db = new Database(TESTING_CONNECTION_STRING);
            var sl = db.GetChannelBanSymbols(channelId);
            Assert.Contains(db.Context.Symbols.First(s => s.SymbolId == channelId).SymbolId, 
                sl.Select(s => s.SymbolId).ToList());
        }

        [Fact]
        public void AddSymbolToBanwordListTest()
        {
            using var db = new Database(TESTING_CONNECTION_STRING);
            var symbolsList = new SymbolsList()
            {
                ListId = 121,
                Title = "AddSymbolToBanwordListTest"
            };
            var symbol = new Symbol()
            {
                SymbolId = 123,
                Content = "AddSymbolToBanwordListTest"
            };
            symbolsList = db.Context.SymbolsLists.Add(symbolsList).Entity;
            symbol = db.Context.Symbols.Add(symbol).Entity;
            Assert.NotNull(symbolsList);
            Assert.NotNull(symbol);
            db.BeginTransaction();
            db.AddSymbolToBanwordList(symbolsList, symbol, false);
            if (!db.CommitAsync().Result)
                throw db.Exception;
            symbolsList = db.Context.SymbolsLists
                .Include(sl => sl.Symbols).AsQueryable()
                .First(sl => sl.ListId == symbolsList.ListId);
            Assert.Contains(symbol, symbolsList.Symbols);

            db.BeginTransaction();
            db.AddSymbolToBanwordList(symbolsList, symbol, true);
            if (!db.CommitAsync().Result)
                throw db.Exception;
            Assert.Contains(symbol, symbolsList.Symbols);
        }

        [Fact]
        public void RemoveSymbolFromSymbolsListTest()
        {
            using var db = new Database(TESTING_CONNECTION_STRING);
            var symbolsList = new SymbolsList()
            {
                ListId = 122,
                Title = "ASTBLTest"
            };
            var symbol = new Symbol()
            {
                SymbolId = 124,
                Content = "ASlLtTSymbol"
            };
            db.Context.SymbolsListsToSymbols.Add(new SymbolsListsToSymbols()
            {
                SymbolNavigation = symbol,
                SymbolsListNavigation = symbolsList,
                IsExcluded = true
            });
            db.Context.SaveChanges();
            Assert.Contains(symbol, 
                db.Context.SymbolsLists.Include(sl => sl.Symbols).AsQueryable()
                .First(sl => sl.ListId == symbolsList.ListId)
                .Symbols.ToList());
            db.BeginTransaction();
            db.RemoveSymbolFromSymbolsList(symbolsList.ListId, symbol.SymbolId);
            if (!db.CommitAsync().Result)
                throw db.Exception;
            Assert.DoesNotContain(symbol,
                db.Context.SymbolsLists.Include(sl => sl.Symbols).AsQueryable()
                .First(sl => sl.ListId == symbolsList.ListId)
                .Symbols.ToList());
        }

        [Fact]
        public void DeleteSymbolsListTest()
        {
            using var db = new Database(TESTING_CONNECTION_STRING);
            var symbolsList = new SymbolsList()
            {
                ListId = 1222,
                Title = "DeleteSymbolsListTest"
            };
            db.Context.SymbolsLists.Add(symbolsList);
            db.Context.SaveChanges();
            db.BeginTransaction();
            db.DeleteSymbolsList(symbolsList);
            if (!db.CommitAsync().Result)
                throw db.Exception;
            Assert.DoesNotContain(symbolsList, db.Context.SymbolsLists.AsQueryable());
        }

        [Fact]
        public void AddSymbolTest()
        {
            using var db = new Database(TESTING_CONNECTION_STRING);
            db.BeginTransaction();
            var symbol = db.AddSymbol("AddSymbolTest");
            if (!db.CommitAsync().Result)
                throw db.Exception;
            Assert.Contains(symbol, db.Context.Symbols.AsQueryable());
        }

        [Fact]
        public void GetSymbolsTest()
        {
            using var db = new Database(TESTING_CONNECTION_STRING);
            var symbolsList = new SymbolsList()
            {
                ListId = 12222,
                Title = "GetSymbolsTest"
            };
            var symbol1 = new Symbol()
            {
                SymbolId = 81,
                Content = "GetSymbolsTestSymbol1"
            };
            var symbol2 = new Symbol()
            {
                SymbolId = 82,
                Content = "GetSymbolsTestSymbol2"
            };
            symbolsList.Symbols.Add(symbol1);
            symbolsList.Symbols.Add(symbol2);
            db.Context.SymbolsLists.Add(symbolsList);
            db.Context.SaveChanges();
            Assert.Contains(symbol1, db.GetSymbols(symbolsList.ListId));
            Assert.Contains(symbol2, db.GetSymbols(symbolsList.ListId));
        }

        [Fact]
        public void FindSymbolTest()
        {
            using var db = new Database(TESTING_CONNECTION_STRING);
            var symbol1 = new Symbol()
            {
                SymbolId = 100,
                Content = "FindSymbolTest"
            };
            db.Context.Symbols.Add(symbol1);
            db.Context.SaveChanges();
            var symbol2 = db.FindSymbol(symbol1.Content);
            Assert.Equal(symbol1.SymbolId, symbol2.SymbolId);
        }

        [Fact]
        public void DeleteSymbolTest()
        {
            using var db = new Database(TESTING_CONNECTION_STRING);
            var symbol = new Symbol()
            {
                SymbolId = 10000,
                Content = "DeleteSymbolTest"
            };
            db.Context.Symbols.Add(symbol);
            db.Context.SaveChanges();
            db.BeginTransaction();
            db.DeleteSymbol(symbol.SymbolId);
            if (!db.CommitAsync().Result)
                throw db.Exception;
            Assert.DoesNotContain(symbol, db.Context.Symbols.AsQueryable());
        }

        [Fact]
        public void GetSymbolsListsToSymbolsTest()
        {
            using var db = new Database(TESTING_CONNECTION_STRING);
            var symbolsList = new SymbolsList()
            {
                ListId = 999999,
                Title = "GetSymbolsListsToSymbolsTestList"
            };
            var symbolsListsToSymbols = new List<Symbol>()
            {
                new Symbol() { Content = "GetSymbolsListsToSymbolsTest1" },
                new Symbol() { Content = "GetSymbolsListsToSymbolsTest2" },
                new Symbol() { Content = "GetSymbolsListsToSymbolsTest3" }
            }.Select(s => new SymbolsListsToSymbols()
            {
                SymbolNavigation = s,
                SymbolsListNavigation = symbolsList
            });
            db.Context.SymbolsListsToSymbols.AddRange(symbolsListsToSymbols);
            db.Context.SaveChanges();
            var dbSymbolsListsToSymbols = db.GetSymbolsListsToSymbols(999999);
            Assert.Equivalent(symbolsListsToSymbols.Select(slts => slts.SymbolNavigation?.Content), 
                dbSymbolsListsToSymbols.Select(dbslts => dbslts.SymbolNavigation?.Content));
        }
    }
}
