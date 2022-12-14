using DatabaseModel;
using DiscordBotDurak.Commands;
using DiscordBotDurak.Data;

namespace DiscordBotDurak.Tests
{
    public class CommandTest
    {
        private const ulong GUILD_ID = 123;
        private const ulong CHANNEL_ID = 123123;
        private const long LIST_ID = 100;

        public bool GenerateTestData()
        {
            using var db = new Database(DatabaseTest.TESTING_CONNECTION_STRING);
            if (db.GetSymbolsList(LIST_ID) is not null)
            {
                return true;
            }
            var guild = db.GetGuild(GUILD_ID) ?? new Guild()
            {
                GuildId = GUILD_ID
            };
            var channel = db.GetChannel(CHANNEL_ID) ?? new Channel()
            {
                ChannelId = CHANNEL_ID,
                GuildNavigation = guild
            };
            var list = new SymbolsList()
            {
                ListId = LIST_ID
            };
            list.Guilds.Add(guild);
            list.Channels.Add(channel);
            db.BeginTransaction();
            db.Context.SymbolsLists.Add(list);
            return db.CommitAsync().Result;
        }

        [Fact]
        public void GenerateTestDataTest()
        {
            Assert.True(GenerateTestData());
        }
    }
}
