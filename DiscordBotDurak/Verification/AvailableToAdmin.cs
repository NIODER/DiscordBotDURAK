using DiscordBotDurak.Data;
using DiscordBotDurak.Enum.BotRoles;

namespace DiscordBotDurak.Verification
{
    internal abstract class AvailableToAdmin : IVerifiable
    {
        public bool Verify(ulong userId, ulong guildId)
        {
            using var db = new Database();
            var user = db.GetUser(guildId, userId);
            return user.Role >= (short)BotRole.Admin;
        }
    }
}
