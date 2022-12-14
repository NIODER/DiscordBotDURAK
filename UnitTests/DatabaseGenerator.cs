using DatabaseModel;
using DiscordBotDurak.Data;
using DiscordBotDurak.Enum.BotRoles;
using System.Xml.Schema;

namespace DiscordBotDurak.Tests
{
    public class DatabaseGenerator
    {
        #region data generating
        // Данные создаются так:
        // ----------------------------------------------
        // Guild 1          |   Guild 2                 |
        // Channel 1, 3     |   Channel 2, 4            |
        // User 1 (owner)   |   User 2 (owner)          |
        // User 3 (admin)   |   User 4 (Moderator)      |
        // User 5 (user)    |   User 6 (user (auto))    |
        // ----------------------------------------------
        // ------------------------------------------------------------------
        //  SymbolId:   | Content: | IsExcuded: | ListId:   | ChannelId:    |
        // -------------|----------|------------|-----------|---------------|
        //  1           |   "1"    |   false    |   1       |   1           |
        //  2           |   "2"    |    true    |   2       |   2           |
        //  3           |   "3"    |    false   |   3       |   3           |
        //  4           |   "4"    |    true    |   4       |   4           |
        //  5           |   "5"    |    false   |   5       |   1           |
        //  6           |   "6"    |    true    |   6       |   2           |
        // ------------------------------------------------------------------

        private bool GenerateTestingData(Database database)
        {
            var guilds = new List<Guild>()
            {
                new Guild()
                {
                    GuildId = 1
                },
                new Guild()
                {
                    GuildId = 2
                }
            };
            var channels = new List<Channel>()
            {
                new Channel()
                {
                    ChannelId = 1,
                    GuildNavigation = guilds[0]
                },
                new Channel()
                {
                    ChannelId = 2,
                    GuildNavigation = guilds[1]
                },
                new Channel()
                {
                    ChannelId = 3,
                    GuildNavigation = guilds[0]
                },
                new Channel()
                {
                    ChannelId = 4,
                    GuildNavigation = guilds[1]
                }
            };
            var users = new List<GuildUser>()
            {
                new GuildUser()
                {
                    UserId = 1,
                    GuildNavigation = guilds[0],
                    Role = (short)BotRole.Owner
                },
                new GuildUser()
                {
                    UserId = 2,
                    GuildNavigation = guilds[1],
                    Role = (short)BotRole.Owner
                },
                new GuildUser()
                {
                    UserId = 3,
                    GuildNavigation = guilds[0],
                    Role = (short)BotRole.Admin
                },
                new GuildUser()
                {
                    UserId = 4,
                    GuildNavigation = guilds[1],
                    Role = (short)BotRole.Moderator
                },
                new GuildUser()
                {
                    UserId = 5,
                    GuildNavigation = guilds[0],
                    Role = (short)BotRole.User
                },
                new GuildUser()
                {
                    UserId = 6,
                    GuildNavigation = guilds[1]
                }
            };
            var symbols = new List<Symbol>();
            for (ulong i = 1; i < 7; i++)
            {
                symbols.Add(new Symbol()
                {
                    SymbolId = i,
                    Content = i.ToString()
                });
            }
            var symbolsListsToSymbols = new List<SymbolsListsToSymbols>();
            for (ulong i = 0; i < 6; i++)
            {
                var list = new SymbolsList()
                {
                    ListId = Convert.ToUInt64(i + 1),
                    Title = (i + 1).ToString()
                };
                list.Guilds.Add(guilds.First(g => g.GuildId == (i % 2) + 1));
                list.Channels.Add(channels.First(c => c.ChannelId == (i % 4) + 1));
                symbolsListsToSymbols.Add(new SymbolsListsToSymbols()
                {
                    SymbolNavigation = symbols.First(s => s.SymbolId == i + 1),
                    SymbolsListNavigation = list,
                    IsExcluded = i % 2 == 0
                });
            }
            database.Context.Guilds.AddRange(guilds);
            database.Context.GuildUsers.AddRange(users);
            database.Context.SymbolsListsToSymbols.AddRange(symbolsListsToSymbols);
            database.Context.SaveChanges();
            return true;
        }
        #endregion

        [Fact]
        public void GenetateTestingDataTest()
        {
            var database = new Database(DatabaseTest.TESTING_CONNECTION_STRING);
            database.Context.Database.EnsureDeleted();
            database.Context.Database.EnsureCreated();
            Assert.True(GenerateTestingData(database));
        }
    }
}
